using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Box
{
    public int ID { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Depth { get; set; }
    public double Weight { get; set; }
    public DateTime ProductionDate { get; set; }

    //Расчет срока годности
    public DateTime ExpiryDate => ProductionDate.AddDays(100);

    //Объем коробки
    public double CalculateVolume()
    {
        return Width * Height * Depth;
    }
}

class Pallet
{
    public int ID { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Depth { get; set; }
    public List<Box> Boxes { get; set; } = new List<Box>();

    public DateTime CalculateExpiryDate()
    {
        if (Boxes.Count > 0)
            return Boxes.Min(box => box.ExpiryDate);
        return DateTime.MaxValue;
    }

    //Расчет весов коробок вместе с паллетой
    public double CalculateWeight()
    {
        return Boxes.Sum(box => box.Weight) + 30;
    }

    //Расчет объема паллеты + объем коробок
    public double CalculateVolume()
    {
        return Boxes.Sum(box => box.CalculateVolume()) + Width * Height * Depth;
    }
}

class Program
{
    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

        List<Pallet> pallets = new List<Pallet>();

        // Получение данных от пользователя
        Console.Write("Введите количество паллет: ");
        int numberOfPallets = int.Parse(Console.ReadLine());

        for (int i = 1; i <= numberOfPallets; i++)
        {
            Pallet pallet = new Pallet();
            pallet.ID = i;

            Console.Write($"Введите ширину паллеты {i}: ");
            pallet.Width = double.Parse(Console.ReadLine());

            Console.Write($"Введите высоту паллеты {i}: ");
            pallet.Height = double.Parse(Console.ReadLine());

            Console.Write($"Введите глубину паллеты {i}: ");
            pallet.Depth = double.Parse(Console.ReadLine());

            Console.Write($"Введите количество коробок на паллете {i}: ");
            int numberOfBoxes = int.Parse(Console.ReadLine());

            for (int j = 1; j <= numberOfBoxes; j++)
            {
                Box box = new Box();

                Console.Write($"Введите ID коробки {j} на паллете {i}: ");
                box.ID = int.Parse(Console.ReadLine());

                Console.Write($"Введите ширину коробки {j} на паллете {i}: ");
                double boxWidth = double.Parse(Console.ReadLine());

                Console.Write($"Введите высоту коробки {j} на паллете {i}: ");
                double boxHeight = double.Parse(Console.ReadLine());

                Console.Write($"Введите глубину коробки {j} на паллете {i}: ");
                double boxDepth = double.Parse(Console.ReadLine());

                Console.Write($"Введите вес коробки {j} на паллете {i}: ");
                double boxWeight = double.Parse(Console.ReadLine());

                Console.Write($"Введите дату производства коробки {j} на паллете {i} (дд.мм.гггг): ");
                DateTime productionDate = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", null);

                //Проверка на размер
                if (boxWidth <= pallet.Width && boxDepth <= pallet.Depth)
                {
                    box.Width = boxWidth;
                    box.Height = boxHeight;
                    box.Depth = boxDepth;
                    box.Weight = boxWeight;
                    box.ProductionDate = productionDate;

                    pallet.Boxes.Add(box);
                }
                else
                {
                    Console.WriteLine($"Коробка {j} на паллете {i} превышает размеры паллеты {i}");
                }
            }

            if (pallet.Boxes.Count > 0)
            {
                pallets.Add(pallet);
            }
        }

        // Группировка паллет по сроку годности и сортировка
        var groupedPallets = pallets
    .SelectMany(pallet => pallet.Boxes.Select(box => new { Pallet = pallet, Box = box }))
    .OrderBy(pair => pair.Pallet.CalculateExpiryDate())
    .ThenBy(pair => pair.Pallet.CalculateWeight())
    .GroupBy(pair => pair.Pallet.CalculateExpiryDate());

        Console.WriteLine("Паллеты, сгруппированные по сроку годности и отсортированные по весу:");

        foreach (var group in groupedPallets)
        {
            var expiryDate = group.Key;
            var sortedPalletsInGroup = group.OrderBy(pair => pair.Pallet.CalculateWeight());

            foreach (var pair in sortedPalletsInGroup)
            {
                var pallet = pair.Pallet;
                var box = pair.Box;
                Console.WriteLine($"Паллет ID: {pallet.ID}, Срок годности: {expiryDate.ToString("d")}, Вес: {pallet.CalculateWeight()} кг");
            }
        }

        var topPallets = pallets
            .OrderByDescending(pallet => pallet.Boxes.Max(box => box.ExpiryDate))
            .Take(3)
            .OrderBy(pallet => pallet.CalculateVolume());

        Console.WriteLine("Три паллеты с наибольшим сроком годности, отсортированные по объему:");

        foreach (var pallet in topPallets)
        {
            Console.WriteLine($"Паллет ID: {pallet.ID}, Срок годности: {pallet.CalculateExpiryDate().ToString("d")}, Объем: {pallet.CalculateVolume()} куб. м");
        }

        Console.ReadLine();
    }
}
