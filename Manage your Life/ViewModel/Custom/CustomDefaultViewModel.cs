using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{
    public class CustomDefaultViewModel : ViewModel
    {

        public CustomDefaultViewModel()
        {
            LoadPalettes();
            Settings();
        }


        public string ChartTitle { get { return "Custom Chart"; } }
        public string ChartSubTitle { get { return "Please set in the left column."; } }
        public string SeriesTitle { get { return ""; } }
    }
}
