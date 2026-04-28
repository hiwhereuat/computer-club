using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace computerclub.Controls
{
    public partial class FilterPanel : UserControl
    {
        public event EventHandler? FiltersChanged;

        public FilterPanel()
        {
            InitializeComponent();

            // Подписываемся на события
            TxtSearch.TextChanged += (s, e) => OnFiltersChanged();
            TxtFilter.TextChanged += (s, e) => OnFiltersChanged();
            NumFrom.TextChanged += (s, e) => OnFiltersChanged();
            NumTo.TextChanged += (s, e) => OnFiltersChanged();
            DateFrom.SelectedDateChanged += (s, e) => OnFiltersChanged();
            DateTo.SelectedDateChanged += (s, e) => OnFiltersChanged();
            CmbBoolValue.SelectionChanged += (s, e) => OnFiltersChanged();
            CmbFilterColumn.SelectionChanged += CmbFilterColumn_SelectionChanged;

            BtnClearSearch.Click += (s, e) => ClearSearch();
            BtnClearFilter.Click += (s, e) => ClearFilter();
        }

        private void CmbFilterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilterPanelVisibility();
            OnFiltersChanged();
        }

        private void UpdateFilterPanelVisibility()
        {
            if (CmbFilterColumn.SelectedItem is not ComboBoxItem item) return;

            var tag = item.Tag?.ToString() ?? "Text";

            // Скрываем все панели
            PanelNumeric.Visibility = Visibility.Collapsed;
            PanelText.Visibility = Visibility.Collapsed;
            PanelDate.Visibility = Visibility.Collapsed;
            PanelBool.Visibility = Visibility.Collapsed;

            // Показываем нужную панель в зависимости от типа
            if (tag.StartsWith("Numeric"))
            {
                PanelNumeric.Visibility = Visibility.Visible;
            }
            else if (tag.StartsWith("Text"))
            {
                PanelText.Visibility = Visibility.Visible;
            }
            else if (tag.StartsWith("Date"))
            {
                PanelDate.Visibility = Visibility.Visible;
            }
            else if (tag.StartsWith("Bool"))
            {
                PanelBool.Visibility = Visibility.Visible;
            }
            else
            {
                // По умолчанию - текстовый фильтр
                PanelText.Visibility = Visibility.Visible;
            }
        }

        public void SetSearchColumns(Dictionary<string, string> columns)
        {
            CmbSearchColumn.Items.Clear();
            foreach (var col in columns)
            {
                CmbSearchColumn.Items.Add(new ComboBoxItem { Content = col.Key, Tag = col.Value });
            }
            if (CmbSearchColumn.Items.Count > 0)
                CmbSearchColumn.SelectedIndex = 0;
        }

        public void SetFilterColumns(Dictionary<string, string> columns)
        {
            CmbFilterColumn.Items.Clear();
            foreach (var col in columns)
            {
                CmbFilterColumn.Items.Add(new ComboBoxItem { Content = col.Key, Tag = col.Value });
            }
            if (CmbFilterColumn.Items.Count > 0)
            {
                CmbFilterColumn.SelectedIndex = 0;
                UpdateFilterPanelVisibility();
            }
        }

        private void OnFiltersChanged()
        {
            FiltersChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSearch()
        {
            TxtSearch.Clear();
        }

        public void ClearFilter()
        {
            NumFrom.Clear();
            NumTo.Clear();
            TxtFilter.Clear();
            DateFrom.SelectedDate = null;
            DateTo.SelectedDate = null;
            CmbBoolValue.SelectedIndex = 0;
        }

        public void ClearAll()
        {
            ClearSearch();
            ClearFilter();
        }

        public string SearchText => TxtSearch.Text;
        public string SearchColumn => (CmbSearchColumn.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";

        public string FilterColumn => (CmbFilterColumn.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        public string FilterText => TxtFilter.Text;
        public decimal? FilterFrom => decimal.TryParse(NumFrom.Text, out var v) ? v : null;
        public decimal? FilterTo => decimal.TryParse(NumTo.Text, out var v) ? v : null;
        public DateTime? FilterDateFrom => DateFrom.SelectedDate;
        public DateTime? FilterDateTo => DateTo.SelectedDate;
        public bool? FilterBoolValue
        {
            get
            {
                if (CmbBoolValue.SelectedItem is ComboBoxItem item && bool.TryParse(item.Tag?.ToString(), out var val))
                    return val;
                return null;
            }
        }
    }
}