using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace PingLib.Models
{
    public class IpListToXml
    {
        [XmlElement("SelectedItemValues")]
        public string SelectedItemValues { get; set; }
        [XmlElement("ItemValues")]
        public List<string> ItemValues { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        public static string DELETE_COMMAND = "Delete All Items";

        /// <summary>
        /// Constructor
        /// </summary>
        public IpListToXml()
        {
            ItemValues = new List<string>();
            FileName = "IpComboBoxList.xml";
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string selectedItemValues, ObservableCollection<string> oc)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IpListToXml));

                SelectedItemValues = selectedItemValues;
                ItemValues = OtoL(oc);
                var XmlWriter = new StreamWriter(FileName);
                serializer.Serialize(XmlWriter, this);
                XmlWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="theResult"></param>
        /// <param name="sc"></param>
        public ObservableCollection<string> Load(ref string selectedItemValues)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IpListToXml));

                ItemValues = new List<string>();
                using (StreamReader rd = new StreamReader(FileName))
                {
                    var xmlImport = serializer.Deserialize(rd) as IpListToXml;
                    SelectedItemValues = xmlImport.SelectedItemValues;
                    if (xmlImport.ItemValues.Count == 0)
                        SetDefaultValues(xmlImport);
                    selectedItemValues = xmlImport.SelectedItemValues;
                    return LtoO(xmlImport.ItemValues);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                SetDefaultValues(this);
                selectedItemValues = SelectedItemValues;
                Save(SelectedItemValues, LtoO(ItemValues));
                return LtoO(ItemValues);
            }
        }

        /// <summary>
        /// SetDefaultValues
        /// </summary>
        public void SetDefaultValues()
        {
            ItemValues.Clear();
            ItemValues.Add(DELETE_COMMAND);
            SelectedItemValues = ItemValues[0];
        }

        /// <summary>
        /// SetDefaultValues
        /// </summary>
        private void SetDefaultValues(IpListToXml iltx)
        {
            iltx.ItemValues.Add(DELETE_COMMAND);
            iltx.SelectedItemValues = iltx.ItemValues[0];
        }

        /// <summary>
        /// OtoL
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        private List<string> OtoL(ObservableCollection<string> oc)
        {
            List<string> l = new List<string>();
            foreach (var i in oc) l.Add(i);
            return l;
        }

        /// <summary>
        /// LtoO
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private ObservableCollection<string> LtoO(List<string> l)
        {
            ObservableCollection<string> oc = new ObservableCollection<string>();
            foreach (var i in l) oc.Add(i);
            return oc;
        }
    }
}
