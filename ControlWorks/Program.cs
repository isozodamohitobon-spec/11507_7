namespace ControlWork;

public class Task1
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class Validator<T>
    {
        public Predicate<T> Rule { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsValid(T value)
        {
            if (Rule == null)
                throw new InvalidOperationException("Правило проверки не задано!");
            return Rule(value);
        }

        public void EnsureValid(T value)
        {
            if (!IsValid(value))
                throw new ValidationException(ErrorMessage ?? "Проверка не пройдена.");
        }
    }

    class Program
    {
        static void Main()
        {
            var positiveValidator = new Validator<int>
            {
                Rule = x => x > 0,
                ErrorMessage = "Числ должен  быть положительным :)"
            };

            
            var stringValidator = new Validator<string>
            
            {
                Rule = s => !string.IsNullOrWhiteSpace(s) && s.Length > 5,
                ErrorMessage = "Строка не должна быть пустой и её длина должна быть больше 5 символов :( "
            };

            // Проверка чисел
            TestValidator(positiveValidator, 5);
            TestValidator(positiveValidator, -3);

            // Проверка строк
            TestValidator(stringValidator, "HelloWorld");
            TestValidator(stringValidator, "Hi Andrey");
            
            Console.ReadKey();
            Console.Clear();
        }

        static void TestValidator<T>(Validator<T> validator, T value)
        {
            Console.Write($"Проверка значения '{value}': ");
            try
            {
                validator.EnsureValid(value);
                Console.WriteLine("Успешно!");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message} ");
            }
        }
    }
}
namespace ControlWork;
    
public class Task2
{
    public class TimerEventArgs : EventArgs
    {
        public int SecondsRemaining { get; set; }
    }

    public class CountDownTimer
    {
        private bool _stopped = false;

        public event EventHandler<TimerEventArgs> Tick;
        public event EventHandler Stopped;

        public void Start(int seconds)
        {
            if (seconds <= 0)
                throw new ArgumentException("Время должно быть положительным числом.");

            _stopped = false;
            Console.WriteLine($"Таймер запущен на {seconds} секунд. Нажмите 'q' для выхода.\n");

            for (int i = seconds; i >= 0; i--)
            {
                if (_stopped) break;

                if (i > 0)
                {
                    OnTick(i);
                }
                else
                {
                    OnStopped();
                    break;
                }

                // Ожидание 1 секунды 
                for (int t = 0; t < 10 && !_stopped; t++)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        _stopped = true;
                        Console.WriteLine("\nТаймер прерван пользователем.");
                        break;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        protected virtual void OnTick(int remaining)
        {
            Tick?.Invoke(this, new TimerEventArgs { SecondsRemaining = remaining });
        }

        protected virtual void OnStopped()
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Задание 2. Обратный отсчет ===\n");

            var timer = new CountDownTimer();
            timer.Tick += (sender, e) => Console.WriteLine($"Осталось: {e.SecondsRemaining} сек.");
            timer.Stopped += (sender, e) => Console.WriteLine("Время вышлo:(  Бум!!!");

            timer.Start(5);

            Console.ReadKey();
            Console.Clear();
        }
    }
}
namespace ControlWork;

public class Task3
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Задание 3. Обработка XML (способ 2: XmlDocument + XPath) ===\n");

            string xmlPath = "orders.xml";
            string reportPath = "report.txt";

            if (!File.Exists(xmlPath))
            {
                CreateSampleXml(xmlPath);
                Console.WriteLine("Создан пример файла orders.xml\n");
            }

            try
            {
               
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

               
                XmlNodeList paidOrders = doc.SelectNodes("/Orders/Order[@Status='Paid']");

                if (paidOrders == null || paidOrders.Count == 0)
                {
                    Console.WriteLine("Заказы со статусом 'Paid' не найдены.");
                    return;
                }

            
                string paidIds = "";
                int totalAmount = 0;

                for (int i = 0; i < paidOrders.Count; i++)
                {
                    XmlNode order = paidOrders[i];

                    // Получаем атрибуты
                    string idStr = order.Attributes?["Id"]?.Value ?? "0";
                    string amountStr = order.Attributes?["Amount"]?.Value ?? "0";

                    int id = int.Parse(idStr);
                    int amount = int.Parse(amountStr);
                    
                    if (i > 0)
                        paidIds += ", ";
                    paidIds += id;

                    totalAmount += amount;
                }

                // 3. Сохраняем результат в report.txt
                using (StreamWriter writer = new StreamWriter(reportPath, false, Encoding.UTF8))
                {
                    writer.WriteLine($"Paid Orders: {paidIds}");
                    writer.WriteLine($"Total Amount: {totalAmount}");
                }

                // Выводим на экран
                Console.WriteLine("Результат обработки:");
                Console.WriteLine($"Paid Orders: {paidIds}");
                Console.WriteLine($"Total Amount: {totalAmount}");
                Console.WriteLine($"\nРезультат сохранён в файл: {Path.GetFullPath(reportPath)}");
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"Ошибка синтаксиса XML: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        static void CreateSampleXml(string path)
        {
            XmlDocument doc = new XmlDocument();
   
            XmlElement root = doc.CreateElement("Orders");
            doc.AppendChild(root);

            // Order 1
            XmlElement order1 = doc.CreateElement("Order");
            order1.SetAttribute("Id", "1");
            order1.SetAttribute("Status", "Paid");
            order1.SetAttribute("Amount", "1500");
            root.AppendChild(order1);

            // Order 2
            XmlElement order2 = doc.CreateElement("Order");
            order2.SetAttribute("Id", "2");
            order2.SetAttribute("Status", "Pending");
            order2.SetAttribute("Amount", "500");
            root.AppendChild(order2);

            // Order 3
            XmlElement order3 = doc.CreateElement("Order");
            order3.SetAttribute("Id", "3");
            order3.SetAttribute("Status", "Paid");
            order3.SetAttribute("Amount", "3000");
            root.AppendChild(order3);

            doc.Save(path);
        }
    }
}

