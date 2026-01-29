using System.Xml.Linq;
using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.App.Views;

public partial class Subject : ContentPage
{
    private int subjectId;
    private bool isDataLoaded = false;

    public Subject()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        var path = Shell.Current.CurrentState.Location.ToString().Trim('/');

        CantoneseSubject subject = await DataProcessor.GetCantoneseSubjectByEnName(path);

        this.subjectId = subject == null ? 0 : subject.Id;

        if (this.subjectId >= 1)
        {
            if (!this.isDataLoaded)
            { 
                this.LoadData();
            }
        }
    }

    private async void LoadData()
    {
        var subject = await DataProcessor.GetCantoneseSubject(this.subjectId);

        if (subject == null)
        {
            return;
        }

        this.lblDescription.Text = subject.Description;
        this.lblDescription.IsVisible = !string.IsNullOrEmpty(subject.Description);

        var medias = await ImageHelper.DecorateMedias(await DataProcessor.GetVCantoneseSubjectMedias(this.subjectId));

        this.lvMedias.ItemsSource = medias;

        this.lblIntroductionTitle.IsVisible = medias.Count() > 0;

        var topics = await DataProcessor.GetCantoneseTopics(this.subjectId);

        this.lblDetailsTitle.IsVisible = topics.Count() > 0;

        this.lvTopics.ItemsSource = topics;

        this.isDataLoaded = true;
    }

    private void TapGestureRecognizer_TopicTapped(object sender, TappedEventArgs e)
    {
        var topic = (sender as View).BindingContext as CantoneseTopic;

        this.ShowTopicDetails(topic);
    }

    private async void ShowTopicDetails(CantoneseTopic topic)
    {
        TopicDetail topicDetail = (TopicDetail)Activator.CreateInstance(typeof(TopicDetail), topic);

        await Navigation.PushAsync(topicDetail);
    }
}