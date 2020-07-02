namespace CropApp.Frontend
{
    public class ImpressumData
    {
        public TMG5 tmg5;
        public RStV rStV;
        public Contact contact;
        public IDs ids;
        
        public string ByFirstName;
        public string BySurName;

        public string Conservancy;

        public ImpressumData() { }

        public ImpressumData(TMG5 tmg5, RStV rStV, Contact contact, IDs ids, string byFirstName, string bySurName, string conservancy)
        {
            this.tmg5 = tmg5;
            this.rStV = rStV;
            this.contact = contact;
            this.ids = ids;
            this.ByFirstName = byFirstName;
            this.BySurName = bySurName;
            this.Conservancy = conservancy;
        }

        public class Contact
        {
            public string FoneAreaCode;
            public string FoneNumber;
            public string FaxAreaCode;
            public string FaxNumber;
            public string EMail;

            public Contact() { }

            public Contact(string foneAreaCode, string foneNumber, string faxAreaCode, string faxNumber, string eMail)
            {
                this.FoneAreaCode = foneAreaCode;
                this.FoneNumber = foneNumber;
                this.FaxAreaCode = faxAreaCode;
                this.FaxNumber = faxNumber;
                this.EMail = eMail;
            }
        }

        public class RStV
        {
            public string FirstName;
            public string SurName;
            public string PLZ;
            public string City;
            public string Adress;
            
            public RStV() { }
            public RStV(string firstName, string surName, string adress, string plz, string city)
            {
                this.FirstName = firstName;
                this.SurName = surName;
                this.PLZ = plz;
                this.City = city;
                this.Adress = adress;
            }
        }
        
        public class TMG5
        {
            public string FirstName;
            public string SurName;
            public string PLZ;
            public string City;
            public string Adress;
            public TMG5() { }

            public TMG5(string firstName, string surName, string adress, string plz, string city)
            {
                this.FirstName = firstName;
                this.SurName = surName;
                this.PLZ = plz;
                this.City = city;
                this.Adress = adress;
            }
        }

        public class IDs
        {
            public string VAT;
            public string Economic;
            public IDs() { }

            public IDs(string vat, string economic)
            {
                this.VAT = vat;
                this.Economic = economic;
            }
        }
        
    }
}