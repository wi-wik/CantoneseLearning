using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using viwik.CantoneseLearning.App.Controls;
using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.Core.Model;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.BLL.MAUI.Manager;
using viwik.CantoneseLearning.Model;
using zoft.MauiExtensions.Core.Extensions;

namespace viwik.CantoneseLearning.App.Views;

public partial class History : ContentPage, INotifyPropertyChanged
{
    public History()
    {
        InitializeComponent();

        this.refreshView.Command = this.RefreshCommand;

        this.LoadData();
    }

    private ICommand RefreshCommand
    {
        get
        {
            return new Command(() =>
        {
            this.refreshView.IsRefreshing = true;
            this.LoadData();
            this.refreshView.IsRefreshing = false;
        });
        }
    }

    private async void LoadData()
    {
        var histories = (await DataProcessor.GetVMediaAccessHistories()).OrderByDescending(item => item.LastAccessTime).ToList();

        var todayHistories = histories.Where(item => item.LastAccessTime.Date == DateTime.Today);
        var yesterdayHistories = histories.Where(item => item.LastAccessTime.Date == DateTime.Today.AddDays(-1));
        var earlierHistories = histories.Where(item => item.LastAccessTime.Date <= DateTime.Today.AddDays(-2));

        List<MediaAccessHistoryGroup> groups = new List<MediaAccessHistoryGroup>();

        if (todayHistories.Count() > 0)
        {
            string name = "今天";

            var items = await ImageHelper.DecorateMedias(todayHistories.Select(item => this.CreateVCantoneseMedia(item, name)).ToList());

            ObservableCollection<CantoneseMediaForEditing> collection = new ObservableCollection<CantoneseMediaForEditing>();
            collection.AddRange(items);

            MediaAccessHistoryGroup group = new MediaAccessHistoryGroup(name, collection);

            groups.Add(group);
        }

        if (yesterdayHistories.Count() > 0)
        {
            string name = "昨天";

            var items = await ImageHelper.DecorateMedias(yesterdayHistories.Select(item => this.CreateVCantoneseMedia(item, name)).ToList());

            ObservableCollection<CantoneseMediaForEditing> collection = new ObservableCollection<CantoneseMediaForEditing>();
            collection.AddRange(items);

            MediaAccessHistoryGroup group = new MediaAccessHistoryGroup(name, collection);

            groups.Add(group);
        }

        if (earlierHistories.Count() > 0)
        {
            string name = "更早";

            var items = await ImageHelper.DecorateMedias(earlierHistories.Select(item => this.CreateVCantoneseMedia(item, null)).ToList());

            ObservableCollection<CantoneseMediaForEditing> collection = new ObservableCollection<CantoneseMediaForEditing>();
            collection.AddRange(items);

            MediaAccessHistoryGroup group = new MediaAccessHistoryGroup(name, collection);

            groups.Add(group);
        }

        bool useGroup = groups.Count > 1;

        this.lvMedias.IsGrouped = useGroup;

        if (groups.Count == 1)
        {
            this.lvMedias.ItemsSource = groups[0];
        }
        else
        {
            this.lvMedias.ItemsSource = groups;
        }

        this.SetToolbarButtonStatus(histories.Count());
    }

    private void SetToolbarButtonStatus(int recordCount)
    {
        var tbiClear = this.ToolbarItems[0];
        var tbiManage = this.ToolbarItems[1];

        tbiClear.Text = recordCount == 0 ? "" : "清空";
        tbiClear.IsEnabled = recordCount > 0;

        tbiManage.Text = recordCount == 0 ? "" : "管理";
        tbiManage.IsEnabled = recordCount > 0;
    }

    private CantoneseMediaForEditing CreateVCantoneseMedia(V_MediaAccessHistory mediaAccessHistory, string name)
    {
        string playTime = mediaAccessHistory.PositionTime;

        var meida = new CantoneseMediaForEditing()
        {
            MediaId = mediaAccessHistory.MediaId,
            MediaTitle = mediaAccessHistory.MediaTitle,
            ImageUrl = mediaAccessHistory.ImageUrl,
            PlatformId = mediaAccessHistory.PlatformId,
            Url = mediaAccessHistory.Url,
            Source = mediaAccessHistory.Source,
            MediaTypeName = this.GetDateTimeDisplayString(name, mediaAccessHistory.LastAccessTime)
        };

        if (!string.IsNullOrEmpty(mediaAccessHistory.Duration))
        {
            TimeSpan position = TimeSpan.Parse(mediaAccessHistory.PositionTime);
            TimeSpan duration = TimeSpan.Parse(mediaAccessHistory.Duration);

            meida.Progress = position.TotalSeconds / duration.TotalSeconds;

            if (position.TotalSeconds >= duration.TotalSeconds)
            {
                playTime = "00:00:00";
            }
        }

        meida.PlayTimes = new List<CantoneseMediaPlayTime>() { new CantoneseMediaPlayTime() { StartTime = playTime } };

        return meida;
    }

    private string GetDateTimeDisplayString(string date, DateTime dateTime)
    {
        return dateTime.ToString($"{(date ?? dateTime.ToString("MM-dd"))} HH:mm");
    }

    private void tbiManage_Clicked(object sender, EventArgs e)
    {
        this.SetControlStatus();
    }

    private void SetControlStatus()
    {
        var tbiClear = this.ToolbarItems[0];
        var tbiManage = this.ToolbarItems[1];

        bool isManage = tbiManage.Text == "管理";

        this.actionGrid.IsVisible = isManage ? true : false;

        this.SetCheckboxesVisible(isManage);

        tbiManage.Text = isManage ? "完成" : "管理";

        tbiClear.Text = isManage ? "" : "清空";
        tbiClear.IsEnabled = !isManage;
    }

    private void SetCheckboxesVisible(bool visible)
    {
        var dataSource = this.lvMedias.ItemsSource;

        if (dataSource is List<MediaAccessHistoryGroup>)
        {
            foreach (MediaAccessHistoryGroup group in dataSource)
            {
                foreach (var item in group)
                {
                    item.IsEditing = visible;
                }
            }
        }
        else
        {
            ObservableCollection<CantoneseMediaForEditing> histories = dataSource as ObservableCollection<CantoneseMediaForEditing>;

            foreach (var item in histories)
            {
                item.IsEditing = visible;
            }
        }
    }

    private void SetCheckboxesSelected(bool selected)
    {
        var controls = (this.lvMedias as View).GetVisualTreeDescendants();

        var dataSource = this.lvMedias.ItemsSource;

        if (dataSource is List<MediaAccessHistoryGroup>)
        {
            foreach (MediaAccessHistoryGroup group in dataSource)
            {
                foreach (var item in group)
                {
                    item.IsSelected = selected;
                }
            }
        }
        else
        {
            ObservableCollection<CantoneseMediaForEditing> histories = dataSource as ObservableCollection<CantoneseMediaForEditing>;

            foreach (var item in histories)
            {
                item.IsEditing = selected;
            }
        }
    }

    private void chkSelectAll_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        this.SetCheckboxesSelected(this.chkSelectAll.IsChecked);
    }

    private async void tbiClear_Clicked(object sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert("询问?", "确定要清空历史记录吗？", "是", "否");

        if (confirmed)
        {
            int affectedRows = await DataProcessor.ClearMediaAccessHistories();

            this.LoadData();

            await DisplayAlert("信息", $"已删除{affectedRows}条历史记录。", "确定");
        }
    }

    private async void btnDelete_Clicked(object sender, EventArgs e)
    {
        List<int> mediaIds = new List<int>();

        int total = 0;

        var dataSource = this.lvMedias.ItemsSource;

        if (dataSource is List<MediaAccessHistoryGroup>)
        {
            foreach (MediaAccessHistoryGroup group in dataSource)
            {
                foreach (var item in group)
                {
                    if (item.IsSelected)
                    {
                        mediaIds.Add(item.MediaId);
                    }

                    total++;
                }
            }
        }
        else
        {
            ObservableCollection<CantoneseMediaForEditing> histories = dataSource as ObservableCollection<CantoneseMediaForEditing>;

            foreach (var item in histories)
            {
                if (item.IsSelected)
                {
                    mediaIds.Add(item.MediaId);
                }

                total++;
            }
        }

        if (mediaIds.Count == 0)
        {
            await DisplayAlert("提示", "请选择要删除的记录！", "确定");
            return;
        }
        else
        {
            bool confirmed = await DisplayAlert("询问?", "确定要删除选择的历史记录吗？", "是", "否");

            if (confirmed)
            {
                int affectedRows = 0;

                if (mediaIds.Count == total)
                {
                    affectedRows = await DataProcessor.ClearMediaAccessHistories();
                }
                else
                {
                    affectedRows = await DataProcessor.DeleteMediaAccessHistoriesByMediaIds(mediaIds);
                }

                this.LoadData();

                this.SetControlStatus();

                await DisplayAlert("信息", $"已删除{affectedRows}条历史记录。", "确定");
            }
        }
    }

    private void lvMedias_ChildAdded(object sender, ElementEventArgs e)
    {
        var element = e.Element;

        if (element is SwipeView)
        {
            var controls = element.GetVisualTreeDescendants();

            foreach (var control in controls)
            {
                if (control is MediaCardView mediaCardView)
                {
                    mediaCardView.ShowProgressBar();
                }
            }
        }
    }

    private async void SwipeItemDelete_Clicked(object sender, EventArgs e)
    {
        SwipeItem swipeItem = sender as SwipeItem;

        CantoneseMediaForEditing media = swipeItem.BindingContext as CantoneseMediaForEditing;

        try
        {
            int affectedRows = await DataProcessor.DeleteMediaAccessHistoriesByMediaIds(new List<int>() { media.MediaId });

            if (affectedRows > 0)
            {
                var dataSource = this.lvMedias.ItemsSource;

                if (dataSource is List<MediaAccessHistoryGroup>)
                {
                    foreach (MediaAccessHistoryGroup group in dataSource)
                    {
                        foreach (var item in group)
                        {
                            if (item.MediaId == media.MediaId)
                            {
                                group.Remove(item);
                            }
                        }
                    }
                }
                else
                {
                    ObservableCollection<CantoneseMediaForEditing> histories = dataSource as ObservableCollection<CantoneseMediaForEditing>;

                    foreach (var item in histories)
                    {
                        if (item.MediaId == media.MediaId)
                        {
                            histories.Remove(item);
                            break;
                        }
                    }
                }
            }
            else
            {
                await DisplayAlert("信息", $"删除失败！", "确定");
            }
        }
        catch (Exception ex)
        {
            LogManager.LogException(ex);

            await DisplayAlert("错误", ex.Message, "确定");
        }
    }
}