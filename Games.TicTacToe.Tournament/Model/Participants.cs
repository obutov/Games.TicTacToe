namespace Games.TicTacToe.Tournament
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Participants
    {

        private Participant[] listField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Participant", IsNullable = false)]
        public Participant[] List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Participant
    {

        private string nameField;

        private string classNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClassName
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }
    }


}
