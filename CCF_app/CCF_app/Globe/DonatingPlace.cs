using System.Windows;

/*
 *  Data object to store information about the donating place on the globe. 
 */

namespace CCF_app.Globe
{
    public class DonatingPlace
    {
        public string Name { get; set; }
        public int AmountOfDonaters { get; set; }
        public Rect Bound { get; set; }
    }
}