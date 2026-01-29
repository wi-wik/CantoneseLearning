using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.Core.Helper;
using viwik.CantoneseLearning.BLL.Core.Model;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.Model;
using MyMediaPlayer = viwik.CantoneseLearning.App.Views.MediaPlayer;

namespace viwik.CantoneseLearning.App.Controls;

public partial class MediaCardView : ContentView
{
    private DateTime? lastTapTime;

    public MediaCardView()
    {
        InitializeComponent();
    } 
  

    private bool CanNavigateToMediaPlayer()
    {
        var bindingContext = this.BindingContext;

        if (bindingContext == null)
        {
            return false;
        }

        if (bindingContext is CantoneseMediaForEditing mediaForEditing)
        {
            if (mediaForEditing.IsEditing)
            {
                return false;
            }
        }

        return true;
    }

    private void TapGestureRecognizer_MediaTapped(object sender, TappedEventArgs e)
    {
        if (this.CanNavigateToMediaPlayer())
        {
            DateTime now = DateTime.Now;

            if (this.lastTapTime.HasValue && (now - this.lastTapTime.Value).TotalMilliseconds < 1000)
            {
                return;
            }

            var currentPage = Shell.Current.CurrentPage;

            string currentPageName = currentPage.ToString();

            if (currentPageName.Contains(nameof(CantoneseLearning.App.Views.MediaPlayer)))
            {
                return;
            }           

            this.lastTapTime = now;

            this.ShowMediaPlayer((sender as View).BindingContext as V_CantoneseMedia);
        }
    }

    private async void ShowMediaPlayer(V_CantoneseMedia media)
    {
        if (media is V_CantoneseSubjectMedia subjectMedia)
        {
            var playTimes = await DataProcessor.GetVCantoneseSubjectMediaPlayTimes(subjectMedia.Id);

            media.PlayTimes = playTimes.Select(item => item as CantoneseMediaPlayTime).ToList();
        }
        else if (media is V_CantoneseTopicDetailMedia topicDetailMedia)
        {
            var playTimes = await DataProcessor.GetVCantoneseTopicDetailMediaPlayTimes(topicDetailMedia.Id);

            media.PlayTimes = playTimes.Select(item => item as CantoneseMediaPlayTime).ToList();
        }
        else if (media is V_CantoneseVocabularyMedia vocabularyMedia)
        {
            var playTimes = await DataProcessor.GetVCantoneseVocabularyMediaPlayTimes(vocabularyMedia.Id);

            media.PlayTimes = playTimes.Select(item => item as CantoneseMediaPlayTime).ToList();
        }
        else if (media is V_CantoneseConsonantMedia consonantMedia)
        {
            var playTimes = await DataProcessor.GetVCantoneseConsonantMediaPlayTimes(consonantMedia.Id);

            media.PlayTimes = playTimes.Select(item => item as CantoneseMediaPlayTime).ToList();
        }
        else if (media is V_CantoneseVowelMedia wowelMedia)
        {
            var playTimes = await DataProcessor.GetVCantoneseVowelMediaPlayTimes(wowelMedia.Id);

            media.PlayTimes = playTimes.Select(item => item as CantoneseMediaPlayTime).ToList();
        }

        MyMediaPlayer player = (MyMediaPlayer)Activator.CreateInstance(typeof(MyMediaPlayer), media);

        await Navigation.PushAsync(player);
    }

    public void ShowProgressBar()
    {
        this.pb.IsVisible = true;
        this.InvalidateLayout();
    }
}


