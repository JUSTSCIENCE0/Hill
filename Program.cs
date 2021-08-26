using System;

namespace WelcomeToHill
{
    class Matrix
    {
        private int size;
        private int module;
        private int indexN = 0;
        private int indexM = 0;

        public long[,] a;

        //конструктор матрицы
        public Matrix(int n, int mod) 
        {
            size = n;
            module = mod;
            a = new long[n, n];
        }

        //загрузка двумерного массива, может и не пригодится
        //но вдруг понадобится
        public void Init(long[,] Numbers)
        {
            if ((Numbers.GetLength(0)==Numbers.GetLength(1)) && (Numbers.GetLength(0)==size))
            {
                for (int i=0; i<size; i++)
                    for (int j=0; j<size; j++)
                    {
                        a[i, j] = Numbers[i, j] % module;
                    }

                indexN = size;
                indexM = size; 
            }
            else
            {
                throw new ArgumentException("Incorrect array!");
            }
        }

        //Дебажные функции
        //да они в середине кода
        //ой все, мой код, че хочу, то и ворочу
        /// ////////////////////////////////////////////////////////////////

        private void InitForDebug()
        {
            a[0, 0] = 9; a[0, 1] = 2; a[0, 2] = 5;
            a[1, 0] = 9; a[1, 1] = 4; a[1, 2] = 6;
            a[2, 0] = 2; a[2, 1] = 6; a[2, 2] = 7;
        }

        public void PrintArray(long[,] m)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(m[i, j].ToString() + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void PrintArray(long[,] m, long[,] n)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(m[i, j].ToString() + "\t");
                }
                Console.Write("|\t");
                for (int j = 0; j < size; j++)
                {
                    Console.Write(n[i, j].ToString() + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //конец дебажных функций
        /// ///////////////////////////////////////////////////////////////////////

        //создание единичной матрицы
        public void InitZero()
        {
            a = new long[size, size];

            for (int i=0; i<size; i++)
            {
                a[i, i] = 1;
            }
        }

        //наполнение матрицы числами одно за другим
        //сделал, чтоб можно было сразу с консоли загонять цифры в матрицу 
        public void AddElement(long Number)
        {
            if ((indexN*size+indexM)<=(size*size))
            {
                a[indexN, indexM] = Number % module;
                indexM++;

                if (indexM==size)
                {
                    indexN++;
                    indexM = 0;
                }
            }
            else
            {
                throw new Exception("Matrix is already full!");
            }
        }

        //матрица заполняемая рандомными числами
        public void InitRandom(int sid)
        {
            Random rnd = new Random((int)(DateTime.Now.Millisecond + sid));

            for (int i=0; i<size; i++)
                for (int j=0; j<size; j++)
                {
                    long n = rnd.Next(module - 1) + 1;
                    a[i, j] = n;
                }

            indexN = size;
            indexM = size;

            if (CalcDet() == 0) InitRandom(0);
        }


        //обратное число по модулю (спизжено из афинного)
        private long FindOpposition(long Number)
        {
            long x2 = 1;
            long x1 = 0;
            long y2 = 0;
            long y1 = 1;
            long q, r, x, y;
            long a = module; long b = Number;

            while (b > 0)
            {
                q = a / b;
                r = a - q * b;
                x = x2 - q * x1;
                y = y2 - q * y1;
                a = b; b = r;
                x2 = x1; x1 = x;
                y2 = y1; y1 = y;
            }
            if (a != 1)
            {
                return -1;
            }
            else
            {
                y = y2;
                if (y > 0)
                {
                    return y;
                }
                else return y + module;
            }
        }


        //деление по модулю С=А/В
        private long Div(long A, long B)
        {
            long C = A * FindOpposition(B);
            if (C < 0)
            {
                return C + module;
            }
            else return C % module;
        }

        //вычитание по модулю C=A-B
        private long Minus(long A, long B)
        {
            long C = A - B;

            while(C<0)
            {
                C += module;
            }

            return C % module;
        }

        //нахождение определителя матрицы по модулю
        public long CalcDet()
        {
            long det = 1;
            long[,] b = new long[size,size];
            Array.Copy(a, b, size*size);

            for(int i=0; i<size; i++)
            {
                long mnj = b[i, i];
                det *= mnj;
                for(int j=i; j<size; j++)
                {
                    b[i, j] = Div(b[i, j], mnj);
                }
                for (int n = 1; n < size - i; n++)  
                {
                    mnj = b[i + n, i];
                    for (int j = i; j < size; j++)
                    {
                        b[i + n, j] = Minus(b[i + n, j], mnj * b[i, j]);
                    }
                }
            }

            while(det<0)
            {
                det += module;
            }

            return det % module;
        }

        //нахождение обратной матрицы по модулю
        public Matrix FindOposMatrix()
        {
            Matrix opos = new Matrix(size, module);
            opos.InitZero();

            long[,] b = new long[size, size];
            Array.Copy(a, b, size * size);

            for (int i = 0; i < size; i++)
            {
                long mnj = b[i, i];
                for (int j = i; j < size; j++)
                {
                    b[i, j] = Div(b[i, j], mnj);
                }
                for (int j = 0; j<size; j++)
                {
                    opos.a[i, j] = Div(opos.a[i, j], mnj);
                }
                for (int n = 1; n < size - i; n++)
                {
                    mnj = b[i + n, i];
                    for (int j = i; j < size; j++)
                    {
                        b[i + n, j] = Minus(b[i + n, j], mnj * b[i, j]);
                    }
                    for (int j = 0; j < size; j++)
                    {
                        opos.a[i + n, j] = Minus(opos.a[i + n, j], mnj * opos.a[i, j]);
                    }
                }
            }

            for (int j = size - 1; j > 0; j--)
            {
                for (int n = j - 1; n >= 0; n--)
                {
                    long mnj = b[n, j];
                    for (int m = 0; m < size; m++)
                    {
                        opos.a[n, m] = Minus(opos.a[n, m], opos.a[j, m] * mnj);
                        b[n, m] = Minus(b[n, m], b[j, m] * mnj);
                    }
                }
            }
            return opos;
        }

        public Matrix MultiplexMatrix(Matrix m)
        {
            if ((size != m.size) || (module != m.module))
            {
                throw new Exception("Matrix must have same properties!");
            }

            Matrix res = new Matrix(size, module);
            for (int i=0; i<size; i++)
            {
                for (int n=0; n<size; n++)
                {
                    long l = 0;
                    for (int j=0; j<size; j++)
                    {
                        l += a[i, j] * m.a[j, n];
                    }
                    res.a[i, n] = l % module;
                }
            }
            return res;
        }

        public long[] MultiplexVector(long[] vector)
        {
            if (vector.Length !=size)
            {
                throw new Exception("Matrix must have same properties!");
            }

            long[] res = new long[size];
            for (int i = 0; i < size; i++)
            {
                long l = 0;
                for (int j = 0; j < size; j++)
                {
                    l += a[i, j] * vector[j];
                }
                res[i] = l % module;
            }

            return res;
        }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //n - размер матрицы, mod - модуль, num - количество блоков, на которое будет биться входной текст
            int n = 3, mod = 1201, num;
            string s, OpenText, ClosedText; // s - вспомогательная переменная
            char[] Open; //массив символов незашифрованных
            char[] Close; //массив символов зашифрованных
            long[] OT = new long[3]; //сюда будут складываться блоки исходного текста
            long[] CT = new long[3]; //сюда будут складываться блоки зашифрованного текста

            Console.WriteLine("Шифр Хилла:\nЗашифрование\n");
            Console.WriteLine("Cгенерировать ключ автоматически? (да/нет)");
            s = Console.ReadLine();
            Matrix m = new Matrix(n, mod);
            Matrix next = new Matrix(n, mod);
            Matrix prev = new Matrix(n, mod);
            Matrix OposM, OposNext, OposPrev;

            if (s == "да")
            {
                m.InitRandom(0);
                Console.WriteLine("Матрица сгенерирована!");
            }
            else
            {
                Console.WriteLine("Введите элементы матрицы...");
                for(int i=0; i<n; i++)
                    for (int j=0; j<n; j++)
                    {
                        s = Console.ReadLine();
                        m.AddElement(Convert.ToInt32(s));
                    }
                if (m.CalcDet()==0)
                {
                    Console.WriteLine("Недопустимая матрица!");
                    Console.WriteLine("Матрица будет сгенерирована автоматически...");
                    m.InitRandom(0);
                }
                Console.WriteLine("Матрица введена!");
            }

            Console.WriteLine("\nКлюч: ");
            m.PrintArray(m.a);

            Console.WriteLine("Введите текст: ");
            OpenText = Console.ReadLine();

            //вычисляем количество блоков
            //если длина строки не кратна трем, то значит будет еще один блок
            num = OpenText.Length / n;
            if (OpenText.Length % n != 0)
            {
                num++;
            }

            //добиваем конец тильдами
            Open = new char[num * n];
            Close = new char[num * n];
            for (int i = 0; i<num*n; i++)
            {
                if (i < OpenText.Length) Open[i] = OpenText[i];
                else Open[i] = '`';
            }

            //собственно шифрование
            //для каждого блока
            for (int i=0; i<num; i++)
            {
                //запихиваем индексы символов в блок
                for(int j=0; j<n; j++)
                {
                    OT[j] = (Int32)Open[i * n + j];
                }
                //умножаем
                CT = m.MultiplexVector(OT);
                //запихиваем в массив символов
                for (int j=0; j<n; j++)
                {
                    Close[i * n + j] = (char)CT[j];
                }
            }
            ClosedText = new string(Close);
            Console.WriteLine("\nЗашифрованный текст:\n" + ClosedText);

            Console.WriteLine("\nРасшифрование:\n");
            OposM = m.FindOposMatrix();
            Console.WriteLine("Обратная матрица к ключу:");
            OposM.PrintArray(OposM.a);

            //для чистоты эксперимента обнуляем и пересчитываем все переменные
            num = ClosedText.Length / n;

            Open = new char[num * n];
            Close = new char[num * n];
            for (int i=0; i< ClosedText.Length; i++)
            {
                Close[i] = ClosedText[i];
            }

            //расшифровывание
            for(int i=0; i<num; i++)
            {
                for (int j=0; j<n; j++)
                {
                    CT[j] = (Int32)Close[i * n + j];
                }
                OT = OposM.MultiplexVector(CT);
                for(int j=0; j<n; j++)
                {
                    Open[i * n + j] = (char)OT[j];
                }
            }

            //выкидываем тильды
            for (int i=num*n - 1; Open[i]=='`'; i--)
            {
                Open[i] = (char)0;
            }

            OpenText = new string(Open);
            Console.WriteLine("\nРасшифрованый текст:\n" + OpenText);

            Console.WriteLine("\nЧтобы перейти к рекурсивному шифру Хилла, нажмите любую клавишу...");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("Рекурсивный шифр Хилла:\nЗашифрование\n");
            Console.WriteLine("Cгенерировать ключи автоматически? (да/нет)");
            s = Console.ReadLine();
            if (s == "да")
            {
                m.InitRandom(0);
                Console.WriteLine("Первая матрица сгенерирована!");
                next.InitRandom(1);
                Console.WriteLine("Вторая матрица сгенерирована!");
            }
            else
            {
                Console.WriteLine("Введите элементы первой матрицы...");
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        s = Console.ReadLine();
                        m.AddElement(Convert.ToInt32(s));
                    }
                if (m.CalcDet() == 0)
                {
                    Console.WriteLine("Недопустимая матрица!");
                    Console.WriteLine("Матрица будет сгенерирована автоматически...");
                    m.InitRandom(2);
                }
                Console.WriteLine("Введите элементы второй матрицы...");
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        s = Console.ReadLine();
                        next.AddElement(Convert.ToInt32(s));
                    }
                if (next.CalcDet() == 0)
                {
                    Console.WriteLine("Недопустимая матрица!");
                    Console.WriteLine("Матрица будет сгенерирована автоматически...");
                    next.InitRandom(0);
                }
                Console.WriteLine("Матрицы введены!");
            }

            //сразу нахожу обратные матрицы, потому что иначе исходные матрицы потом потеряются и будет считаться хрень всякая
            OposM = m.FindOposMatrix();
            OposNext = next.FindOposMatrix();

            Console.WriteLine("\nКлюч[1]: ");
            m.PrintArray(m.a);
            Console.WriteLine("\nКлюч[2]: ");
            next.PrintArray(next.a);

            Console.WriteLine("Введите текст: ");
            OpenText = Console.ReadLine();

            num = OpenText.Length / n;
            if (OpenText.Length % n != 0)
            {
                num++;
            }

            Open = new char[num * n];
            Close = new char[num * n];
            for (int i = 0; i < num * n; i++)
            {
                if (i < OpenText.Length) Open[i] = OpenText[i];
                else Open[i] = '`';
            }

            for (int i = 0; i < num; i++)
            {
                //запихиваем индексы символов в блок
                for (int j = 0; j < n; j++)
                {
                    OT[j] = (Int32)Open[i * n + j];
                }
                //умножаем
                if (i == 0)
                {
                    CT = m.MultiplexVector(OT);
                }
                else if(i == 1)
                {
                    CT = next.MultiplexVector(OT);
                }
                else
                {
                    prev = m;
                    m = next;
                    next = m.MultiplexMatrix(prev);
                    CT = next.MultiplexVector(OT);
                }
                //запихиваем в массив символов
                for (int j = 0; j < n; j++)
                {
                    Close[i * n + j] = (char)CT[j];
                }
            }
            ClosedText = new string(Close);
            Console.WriteLine("\nЗашифрованный текст:\n" + ClosedText);

            Console.WriteLine("\nРасшифрование:\n");
            Console.WriteLine("Обратная матрица к ключу[1]:");
            OposM.PrintArray(OposM.a);
            Console.WriteLine("Обратная матрица к ключу[2]:");
            OposNext.PrintArray(OposNext.a);




            num = ClosedText.Length / n;

            Open = new char[num * n];
            Close = new char[num * n];
            for (int i = 0; i < ClosedText.Length; i++)
            {
                Close[i] = ClosedText[i];
            }

            //расшифровывание
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    CT[j] = (Int32)Close[i * n + j];
                }
                if (i == 0)
                {
                    OT = OposM.MultiplexVector(CT);
                }
                else if (i==1)
                {
                    OT = OposNext.MultiplexVector(CT);
                }
                else
                {
                    OposPrev = OposM;
                    OposM = OposNext;
                    OposNext = OposPrev.MultiplexMatrix(OposM);
                    OT = OposNext.MultiplexVector(CT);
                }
                for (int j = 0; j < n; j++)
                {
                    Open[i * n + j] = (char)OT[j];
                }
            }

            //выкидываем тильды
            for (int i = num * n - 1; Open[i] == '`'; i--)
            {
                Open[i] = (char)0;
            }

            OpenText = new string(Open);
            Console.WriteLine("\nРасшифрованый текст:\n" + OpenText);

            Console.ReadKey();
        }

    }
}
