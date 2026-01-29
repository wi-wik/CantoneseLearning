using viwik.CantoneseLearning.BLL.Core;
using viwik.CantoneseLearning.BLL.Core.Model;
using viwik.CantoneseLearning.BLL.MAUI.Helper;
using viwik.CantoneseLearning.BLL.MAUI.Manager;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.App.Views;

public partial class PhoneticAlphabetDetail : ContentPage
{
    private SettingInfo setting;
    public object Content { get; set; }   


    public PhoneticAlphabetDetail()
    {
        InitializeComponent();

        this.setting = SettingManager.GetSetting();

        this.LoadData();
    }

    public PhoneticAlphabetDetail(object content)
    {
        InitializeComponent();

        this.Content = content;

        this.setting = SettingManager.GetSetting();

        this.LoadData();
    }

    private async void LoadData()
    {
        if(this.Content == null)
        {
            return;
        }       

        if (this.Content is V_CantoneseConsonant_YPGP consonant)
        {
            string c = this.setting.PinYinMode == PinYinMode.GP ? consonant.Consonant_GP : consonant.Consonant_YP;       
            string description = this.setting.PinYinMode == PinYinMode.GP ? consonant.Description_GP : consonant.Description_YP;

            this.Title = "¸¨Òô£º" + c;
            this.lblDescription.Text = description;
            this.lblDescription.IsVisible = !string.IsNullOrEmpty(description);

            var medias = await DataProcessor.GetVCantoneseConsonantMedias(consonant.Id);

            this.lvMedias.ItemsSource = medias;
        }
        else if (this.Content is V_CantoneseVowel_YPGP vowel)
        {
            string v = this.setting.PinYinMode == PinYinMode.GP ? vowel.Vowel_GP : vowel.Vowel_YP;
            string description = this.setting.PinYinMode == PinYinMode.GP ? vowel.Description_GP : vowel.Description_YP;

            this.Title = "ÔªÒô£º" + v;
            this.lblDescription.Text = description;
            this.lblDescription.IsVisible = !string.IsNullOrEmpty(description);

            var medias = await ImageHelper.DecorateMedias(await DataProcessor.GetVCantoneseVowelMedias(vowel.Id));             

            this.lvMedias.ItemsSource = medias;
        }
    }
}