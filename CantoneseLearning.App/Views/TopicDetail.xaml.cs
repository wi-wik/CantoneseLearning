using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.Core.Model;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.App.Views;

public partial class TopicDetail : ContentPage
{
    private CantoneseTopic topic;
    public CantoneseTopic Topic
    {
        get => topic;
    }

    public TopicDetail()
    {
        InitializeComponent();
    }

    public TopicDetail(CantoneseTopic topic)
    {
        InitializeComponent();

        this.topic = topic;

        if (this.picker.Items.Count > 0)
        {
            this.picker.SelectedIndex = 0;
        }       

        this.ShowTopicDetails();
    }

    private void tbiShowSearchControl_Clicked(object sender, EventArgs e)
    {
        bool isVisible = this.txtKeyword.IsVisible;

        this.picker.IsVisible = this.txtKeyword.IsVisible = this.btnSearch.IsVisible = !isVisible;
    }

    private void OnSearchButtonClicked(object sender, EventArgs e)
    {
        this.Search();
    }

    private void txtKeyword_Completed(object sender, EventArgs e)
    {
        this.Search();
    }

    private async void ShowTopicDetails()
    {
        if (this.topic == null)
        {
            return;
        }        

        this.Title = this.topic.Name;

        this.Search();
    }  

    private async void Search()
    {
        string keyword = this.txtKeyword.Text?.Trim();

        string filterBy = this.picker.SelectedItem?.ToString();

        bool isFilterByBoth = filterBy == "二者都";

        bool isFilterByTopic = filterBy == "按栏位" || isFilterByBoth;
        bool isFilterByTitle = filterBy == "按标题" || isFilterByBoth;

        var topicDetails = await DataProcessor.GetCantoneseTopicDetails(this.topic.Id, isFilterByTopic?  keyword: null);

        var topicDetailMedias = await ImageHelper.DecorateMedias(await DataProcessor.GetVCantoneseTopicDetailMedias(this.topic.Id, isFilterByTitle? keyword: null));

        List<CantoneseTopicDetailMediaGroup> groups = new List<CantoneseTopicDetailMediaGroup>();

        foreach (var topicDetail in topicDetails)
        {
            var medias = topicDetailMedias.Where(item => item.TopicDetailId == topicDetail.Id).ToList();

            if (medias.Count > 0)
            {
                CantoneseTopicDetailMediaGroup group = new CantoneseTopicDetailMediaGroup(topicDetail.Name, medias) { Description = topicDetail.Description };

                groups.Add(group);
            }
        }

        this.lvMedias.ItemsSource = groups;
    }

    private void TapGestureRecognizer_MediaTapped(object sender, TappedEventArgs e)
    {
        this.ShowMediaPlayer((sender as View).BindingContext as V_CantoneseMedia);
    }

    private async void ShowMediaPlayer(V_CantoneseMedia media)
    {
        MediaPlayer player = (MediaPlayer)Activator.CreateInstance(typeof(MediaPlayer), media);

        await Navigation.PushAsync(player);
    }
}