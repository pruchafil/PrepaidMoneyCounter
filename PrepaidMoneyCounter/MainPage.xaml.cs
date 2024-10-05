using Autofac;
using PrepaidMoneyCounter.Model;
using PrepaidMoneyCounter.ViewModel;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static PrepaidMoneyCounter.ViewModel.MainViewModel;

namespace PrepaidMoneyCounter
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel { get; set; }
        private bool _ignoreAddTransaction = false;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = App.Container.Resolve<MainViewModel>();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_ignoreAddTransaction)
            {
                return;
            }

            _ignoreAddTransaction = true;

            var options = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            double margin = 10d;
            double padding = 10d;

            var received = new Button
            {
                Content = "+",
                Foreground = new SolidColorBrush(Colors.Green),
                FontSize = 20d,
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(10d)
            };

            received.Click += ReceivedMoney_Click;

            var cost = new Button
            {
                Content = "-",
                Foreground = new SolidColorBrush(Colors.Red),
                FontSize = 20d,
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(10d)
            };

            cost.Click += CostMoney_Click;

            options.Children.Add(received);
            options.Children.Add(cost);

            stackPanel.Children.Insert(stackPanel.Children.Count - 1, options);
        }

        private void GetInfoForm(RecordType recordType)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            grid.RowDefinitions.Add(new RowDefinition());

            double margin = 10d;
            double padding = 2d;

            var date = new DatePicker
            {
                Date = DateTime.Now,
                SelectedDate = DateTime.Now,
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(5d)
            };
            var cost = new TextBox
            {
                Text = 0m.ToString(),
                PlaceholderText = "částka",
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(5d)
            };

            cost.BeforeTextChanging += (sender, e) =>
            {
                e.Cancel = !decimal.TryParse(e.NewText, out _);
            };

            var info = new TextBox
            {
                PlaceholderText = "popis",
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(5d)
            };

            var submit = new Button
            {
                Content = "Potvrdit",
                Padding = new Thickness(padding, padding, padding, padding),
                Margin = new Thickness(margin, margin, margin, margin),
                CornerRadius = new CornerRadius(5d)
            };

            submit.Click += async (sender, e) =>
            {
                var record = new Record
                {
                    DateTime = date.Date.Date,
                    Cost = decimal.Parse(cost.Text),
                    RecordType = recordType,
                    Message = info.Text
                };

                await ViewModel.AddRecord(record);
                _ignoreAddTransaction = false;
                stackPanel.Children.RemoveAt(stackPanel.Children.Count - 2);
                await ViewModel.Reload();
                balanceLabel.Text = ViewModel.Balance.ToString();
                balanceLabel.Foreground = ViewModel.BalanceColor;
                dataGrid.ItemsSource = ViewModel.RecordsVm;
            };

            Grid.SetColumn(date, 0);
            Grid.SetColumn(cost, 1);
            Grid.SetColumn(info, 2);
            Grid.SetColumn(submit, 3);
            Grid.SetRow(date, 0);
            Grid.SetRow(cost, 0);
            Grid.SetRow(info, 0);
            Grid.SetRow(submit, 0);

            grid.Children.Add(date);
            grid.Children.Add(cost);
            grid.Children.Add(info);
            grid.Children.Add(submit);
            stackPanel.Children.Insert(stackPanel.Children.Count - 1, grid);
        }

        private void CostMoney_Click(object sender, RoutedEventArgs e)
        {
            stackPanel.Children.RemoveAt(stackPanel.Children.Count - 2);
            GetInfoForm(RecordType.Cost);
        }

        private void ReceivedMoney_Click(object sender, RoutedEventArgs e)
        {
            stackPanel.Children.RemoveAt(stackPanel.Children.Count - 2);
            GetInfoForm(RecordType.Received);
        }

        private async void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            RecordVm obj = ((FrameworkElement)sender).DataContext as RecordVm;
            await ViewModel.RemoveRecord(obj.Guid);
            await ViewModel.Reload();
            dataGrid.ItemsSource = ViewModel.RecordsVm;
            balanceLabel.Text = ViewModel.Balance.ToString();
            balanceLabel.Foreground = ViewModel.BalanceColor;
        }
    }
}
