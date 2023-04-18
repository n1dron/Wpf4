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
            ogo_calendar.SelectedDate = DateTime.Now.Date;
        }
        private int GetFreeNoteId()
        {
            List<Note> notes = JsonWorking.Deserializing<List<Note>>("../../../notes.json");
            int freeId = 1;
            notes = notes.OrderBy(x => x.id).ToList();
            Note lastNote = notes.LastOrDefault();
            if (lastNote != null)
            {
                freeId = lastNote.id + 1;
            }
            else
            {
                freeId = 1;
            }
            return freeId;
        }
        private void RefreshWindow()
        {
            typesList.SelectedIndex = -1;
            nameInput.Text = "";
            countInput.Text = "";
        }
        private List<Note> GetTodayNotes(List<Note> notes, out double todaySumm, out double allSumm)
        {
            todaySumm = 0;
            allSumm = 0;
            DateTime date = Convert.ToDateTime(ogo_calendar.SelectedDate);
            List<Note> todayNotes = new List<Note>();
            foreach (Note note in notes)
            {
                allSumm += note.count;
                if (note.date.Date == date.Date)
                {
                    todaySumm += note.count;
                    todayNotes.Add(note);
                }
            }
            return todayNotes;
        }
        private void UpdateNotes(Note newNote = null)
        {
            string notePath = "../../../notes.json";
            List<Note> notes = JsonWorking.Deserializing<List<Note>>(notePath);
            double todaySumm;
            double allSumm;
            if (newNote != null)
            {
                notes.Add(newNote);
                JsonWorking.Serializing(notePath, notes);
            }
            notesContainer.ItemsSource = GetTodayNotes(notes, out todaySumm, out allSumm);
            allSum.Text = $"Общая сумма: {allSumm}";
            todaySum.Text = $"Общая сумма: {todaySumm}";

        }
        private void UpdateTypesList(string newVal = "")
        {
            string typePath = "../../../types.json";
            List<string> types = new List<string>();
            types = JsonWorking.Deserializing<List<string>>(typePath);
            if (newVal != "")
            {
                types.Add(newVal);
                JsonWorking.Serializing(typePath, types);
            }
            typesList.ItemsSource = types;
        }
        private void DeleteData()
        {
            int selectedId = ((Note)notesContainer.SelectedItem).id; 
            string notePath = "../../../notes.json";
            List<Note> notes = JsonWorking.Deserializing<List<Note>>(notePath);
            List<Note> newNotes = new List<Note>();
            foreach (Note note in notes)
            {
                if (note.id != selectedId)
                {
                    newNotes.Add(note);
                }
            }
            JsonWorking.Serializing(notePath, newNotes);
        }
        private void EditData()
        {
            int selectedId = ((Note)notesContainer.SelectedItem).id;
            string[] values;
            if (typesList.SelectedValue != null)
                values = new string[] { nameInput.Text, countInput.Text, typesList.SelectedValue.ToString() };
            else
                values = new string[] { nameInput.Text, countInput.Text, "" }; 
            if (values.Contains("") || values.Contains(null))
            {
                MessageBox.Show("Вам нужно заполнить все поля");
            }
            else
            {
                try
                {
                    Note newNote = new Note(selectedId, values[0], values[2], Convert.ToDouble(values[1]), Convert.ToDateTime(ogo_calendar.SelectedDate));
                    DeleteData();
                    UpdateNotes(newNote);
                }                         
                catch (Exception)
                {
                    MessageBox.Show($"Похоже, вы ввели неверные данные");
                }
            }
        }
        private void SaveData()
        {                                                                          
            string[] values = new string[] { nameInput.Text, countInput.Text, (typesList.SelectedValue != null ? typesList.SelectedValue.ToString() : "") };
            if (values.Contains("") || values.Contains(null))
            {
                MessageBox.Show("Вам нужно заполнить все поля");
            }
            else
            {
                try
                {
                    Note newNote = new Note(GetFreeNoteId(), values[0], values[2].ToString(), Convert.ToDouble(values[1]), Convert.ToDateTime(ogo_calendar.SelectedDate));
                    UpdateNotes(newNote);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Похоже, вы ввели неверные данные");
                }
            }
        }
        private void createDatatype_Click(object sender, RoutedEventArgs e)
        {
            TypeInput typeInput = new TypeInput();
            typeInput.ShowDialog();
            if (typeInput.isResponse)
            {
                string inputed = typeInput.typedName;
                UpdateTypesList(inputed);
            }
        }
        private void dateUpdated(object sender, EventArgs e)
        {
            UpdateNotes();
            RefreshWindow();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            RefreshWindow();
        }

        private void notesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notesContainer.SelectedIndex == -1) { return; }
            Note selectedNote = (Note)notesContainer.SelectedItem;
            nameInput.Text = selectedNote.name;
            countInput.Text = selectedNote.count.ToString();
            typesList.SelectedIndex = -1;
        }
        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            EditData();
            RefreshWindow();
        }

        private void RemoveNote_Click(object sender, RoutedEventArgs e)
        {
            DeleteData();
            RefreshWindow();
        }
    }
}

