using System.Windows.Controls;
using System.Windows;
using SystemWatch.Repositorys;
using SystemWatch.ViewModels;

namespace SystemWatch.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly StatisticsRepository _statisticsRepository = new();
        
        public StatisticsWindow()
        {
            StatisticsViewModel viewModel = new StatisticsViewModel();
            _statisticsRepository.LoadDayDatas(viewModel);
            DataContext = viewModel;
            InitializeComponent();
        }
        
        private void TimePeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimePeriodComboBox == null) return;
            if (TimePeriodComboBox.SelectedItem is ComboBoxItem selectedItem && DataContext is StatisticsViewModel viewModel)
            {
                string selectedContent = selectedItem.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedContent))
                {
                    switch (selectedContent)
                    {
                        case "1小时":
                            viewModel.TimePeriodType = TimePeriodType.Minutes;
                            viewModel.TimePeriod = 60;
                            break;
                        case "1天":
                            viewModel.TimePeriodType = TimePeriodType.Hours;
                            viewModel.TimePeriod = 24;
                            break;
                        case "1月":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 30;
                            break;
                        case "3月":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 90;
                            break;
                        case "6月":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 180;
                            break;
                        case "1年":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 365;
                            break;
                        case "2年":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 365 * 2;
                            break;
                        case "3年":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 365 * 3;
                            break;
                        case "5年":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 365 * 5;
                            break;
                        case "10年":
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 365 * 10;
                            break;
                        default:
                            viewModel.TimePeriodType = TimePeriodType.Days;
                            viewModel.TimePeriod = 30;
                            break;
                    }
                }
                else
                {
                    viewModel.TimePeriodType = TimePeriodType.Days;
                    viewModel.TimePeriod = 30;
                }
                _statisticsRepository.LoadData(viewModel);
            }
        }
    }
}
