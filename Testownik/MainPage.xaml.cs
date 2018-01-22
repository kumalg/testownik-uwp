﻿using Testownik.Model;
using Testownik.Model.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Testownik
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public string appName = AppIdentity.AppName;

        public TestController testController;
        public TestController TestController {
            get => testController;
            set {
                testController = value;
                RaisePropertyChanged(nameof(TestController));
            }
        }

        public KeyValuePair<string, IQuestion> textQuestion;
        public KeyValuePair<string, IQuestion> TextQuestion {
            get => textQuestion;
            set {
                textQuestion = value;
                RaisePropertyChanged(nameof(TextQuestion));
            }
        }

        public int reoccurrencesOfQuestion;
        public int ReoccurrencesOfQuestion {
            get => reoccurrencesOfQuestion;
            set {
                reoccurrencesOfQuestion = value;
                RaisePropertyChanged(nameof(ReoccurrencesOfQuestion));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            Co();
        }

        private async void Co()
        {
            var co = await QuestionsReader.ReadQuestions();
            TestController = new TestController(co);
            TextQuestion = TestController.RandQuestion();
        }

        private void ButtonNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            var selectedAnswers = AnswersGridView.SelectedItems.Cast<IAnswer>().Select(i => i.Key);
            TestController.CheckAnswer(TextQuestion.Key, selectedAnswers);
            TextQuestion = TestController.RandQuestion();
            ReoccurrencesOfQuestion = TestController.ReoccurrencesOfQuestion(TextQuestion.Key);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}