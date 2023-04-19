using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateTypesList();
            Datochka.SelectedDate = DateTime.Now.Date;
        }
        private void dateUpdated(object sender, EventArgs e)
        {
            UpdateRecord();
            RefreshWindow();
        }
        private void RecordContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordContainer.SelectedIndex == -1) { return; }
            Record selectedNote = (Record)RecordContainer.SelectedItem;
            NameInput.Text = selectedNote.Name;
            MoneyInput.Text = selectedNote.Money.ToString();
            typesList.SelectedIndex = -1;
        }
        private void createDatatype_Click(object sender, RoutedEventArgs e)
        {
            Input typeInput = new Input();
            typeInput.ShowDialog();
            if (typeInput.isResponse)
            {
                string inputed = typeInput.typedName;
                UpdateTypesList(inputed);
            }
        }
        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            RefreshWindow();
        }

        private void EditRecord_Click(object sender, RoutedEventArgs e)
        {
            EditData();
            RefreshWindow();
        }

        private void RemoveRecord_Click(object sender, RoutedEventArgs e)
        {
            DeleteData();
            RefreshWindow();
        }
        private int GetFreeRecordId()
        {
            List<Record> rec = Json.Deserializing<List<Record>>("../../../rec.json");
            int Id = 1;
            rec = rec.OrderBy(x => x.id).ToList();
            Record lastRec = rec.LastOrDefault();
            if (lastRec != null)
            {
                Id = lastRec.id + 1;
            }
            else
            {
                Id = 1;
            }
            return Id;
        }
        private void RefreshWindow()
        {
            typesList.SelectedIndex = -1;
            NameInput.Text = "";
            MoneyInput.Text = "";
        }
        private List<Record> GetTodayRecord(List<Record> rec, out double todaySumm, out double allSumm)
        {
            todaySumm = 0;
            allSumm = 0;
            DateTime date = Convert.ToDateTime(Datochka.SelectedDate);
            List<Record> todayrec = new List<Record>();
            foreach (Record recor in rec)
            {
                allSumm += recor.Money;
                if (recor.Date.Date == date.Date)
                {
                    todaySumm += recor.Money;
                    todayrec.Add(recor);
                }
            }
            return todayrec;
        }
        private void UpdateRecord(Record newrec = null)
        {
            string recPath = "../../../rec.json";
            List<Record> rec = Json.Deserializing<List<Record>>(recPath);
            double todaySumm;
            double allSumm;
            if (newrec != null)
            {
                rec.Add(newrec);
                Json.Serializing(recPath, rec);
            }
            RecordContainer.ItemsSource = GetTodayRecord(rec, out todaySumm, out allSumm);
            allSum.Text = $"Общая сумма: {allSumm}";
            todaySum.Text = $"Общая сумма: {todaySumm}";

        }
        private void UpdateTypesList(string newVal = "")
        {
            string typePath = "../../../types.json";
            List<string> types = new List<string>();
            types = Json.Deserializing<List<string>>(typePath);
            if (newVal != "")
            {
                types.Add(newVal);
                Json.Serializing(typePath, types);
            }
            typesList.ItemsSource = types;
        }
        private void DeleteData()
        {
            int selectedId = ((Record)RecordContainer.SelectedItem).id; 
            string recPath = "../../../rec.json";
            List<Record> rec = Json.Deserializing<List<Record>>(recPath);
            List<Record> newrec = new List<Record>();
            foreach (Record recor in rec)
            {
                if (recor.id != selectedId)
                {
                    newrec.Add(recor);
                }
            }
            Json.Serializing(recPath, newrec);
        }
        private void EditData() 
        {
            int selectedId = ((Record)RecordContainer.SelectedItem).id;
            string[] values;
            if (typesList.SelectedValue != null)
                values = new string[] { NameInput.Text, MoneyInput.Text, typesList.SelectedValue.ToString() };
            else
                values = new string[] { NameInput.Text, MoneyInput.Text, "" }; 
            if (values.Contains("") || values.Contains(null))
            {
                MessageBox.Show("Вам нужно заполнить все поля");
            }
            else
            {
                try
                {
                    Record newrec = new Record(selectedId, values[0], values[2], Convert.ToDouble(values[1]), Convert.ToDateTime(Datochka.SelectedDate));
                    DeleteData();
                    UpdateRecord(newrec);
                }                         
                catch (Exception)
                {
                    MessageBox.Show($"Похоже, вы ввели неверные данные");
                }
            }
        }
        private void SaveData()
        {                                                                          
            string[] values = new string[] { NameInput.Text, MoneyInput.Text, (typesList.SelectedValue != null ? typesList.SelectedValue.ToString() : "") };
            if (values.Contains("") || values.Contains(null))
            {
                MessageBox.Show("Вам нужно заполнить все поля");
            }
            else
            {
                try
                {
                    Record newrec = new Record(GetFreeRecordId(), values[0], values[2].ToString(), Convert.ToDouble(values[1]), Convert.ToDateTime(Datochka.SelectedDate));
                    UpdateRecord(newrec);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Похоже, вы ввели неверные данные");
                }
            }
        }
    }
}

