using System.Windows.Input;
using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.Business.Model;

namespace viwik.CantoneseLearning.App.Views;

public partial class Favorite : ContentPage
{
	public Favorite()
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
        var favorites = await ImageHelper.DecorateMedias(((await DataProcessor.GetVMediaFavorites()).OrderBy(item=>item.CategoryId).ThenByDescending(item => item.CreateTime).ToList()));       

        List<MediaFavoriteGroup> groups = new List<MediaFavoriteGroup>();

        var favoriteGroups = (from item in favorites
                             group item by new { item.CategoryId, item.CategoryName } into g
                             select new { CategoryName = g.Key.CategoryName, CategoryId = g.Key.CategoryId, Values = g })
                             .OrderBy(item=>item.CategoryId).ToList();

        foreach (var fg in favoriteGroups)
        {
            MediaFavoriteGroup group = new MediaFavoriteGroup(fg.CategoryName, fg.Values.ToList());

            groups.Add(group);
        }

        this.lvMedias.ItemsSource = groups;        
    }
}