using System.ComponentModel;
using System.Runtime.CompilerServices;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.App.Model
{
    public class MediaFavoriteCategoryForEditing: MediaFavoriteCategory, INotifyPropertyChanged
    {
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
