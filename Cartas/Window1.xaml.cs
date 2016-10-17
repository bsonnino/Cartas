using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cartas
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            mesa.PreviewMouseLeftButtonDown += mesa_PreviewMouseLeftButtonDown;
            mesa.PreviewMouseMove += mesa_PreviewMouseMove;
            mesa.PreviewMouseLeftButtonUp += mesa_PreviewMouseLeftButtonUp;
            PreviewKeyDown += window1_PreviewKeyDown;
        }

        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private FrameworkElement _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private int posicaoMarcador;

        void window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        void mesa_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool cancelled)
        {
            Mouse.Capture(null);
            if (_originalElement == marcador)
            {
                ReposicionaMarcador(cancelled);
            }
            else
            {
                if (cancelled)
                {
                    ReposicionaCarta(_originalElement);
                }
                else
                {
                    PosicionaCarta();
                }
            }
            _isDragging = false;
            _isDown = false;
        }

        private void ReposicionaMarcador(bool cancelled)
        {
            if (!cancelled)
            {
                Point posicaoAtual = Mouse.GetPosition(mesa);
                posicaoMarcador = Convert.ToInt32(Math.Round((posicaoAtual.X - 1) / 70));
            }
            Canvas.SetLeft(marcador, posicaoMarcador * 70);
            Canvas.SetTop(marcador, 90);
        }

        private void PosicionaCarta()
        {
            Point posicaoAtual = Mouse.GetPosition(mesa);
            bool achouPosicao = false;

            if (posicaoAtual.Y >= 93 && posicaoAtual.Y <= 181)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (posicaoAtual.X >= 3 + 70 * i && posicaoAtual.X <= 71 + 70 * i)
                    {
                        if (!TemCarta(i))
                        {
                            Canvas.SetTop(_originalElement, 95);
                            Canvas.SetLeft(_originalElement, 5 + i * 70);
                            achouPosicao = true;
                        }
                        break;
                    }
                }
            }
            if (!achouPosicao)
                ReposicionaCarta(_originalElement);
        }

        private bool TemCarta(int num)
        {
            for (var i = 2; i <= 8; i++)
            {
                var carta = (Carta)mesa.FindName(string.Format("carta{0}", i));
                if (Canvas.GetTop(carta) == 95 && Canvas.GetLeft(carta) == 5 + num * 70)
                    return true;
            }
            return false;
        }

        private static void ReposicionaCarta(FrameworkElement carta)
        {
            if (carta != null)
            {
                Canvas.SetTop(carta, 3);
                Canvas.SetLeft(carta, Convert.ToInt32(carta.Tag) * 70 + 5);
            }
        }

        void mesa_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(mesa).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(mesa).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

        }

        private void DragMoved()
        {
            Point CurrentPosition = Mouse.GetPosition(mesa);

            Canvas.SetLeft(_originalElement, _originalLeft + CurrentPosition.X - _startPoint.X);
            Canvas.SetTop(_originalElement, _originalTop + CurrentPosition.Y - _startPoint.Y);

        }

        void mesa_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.Source).Name.StartsWith("carta") ||
                  e.Source == marcador)
            {
                _isDown = true;
                _startPoint = e.GetPosition(mesa);
                _originalElement = e.Source as FrameworkElement;
                mesa.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var numeros = new bool[7];
            for (var i = 0; i < 7; i++)
            {
                numeros[i] = false;
            }

            var rnd = new Random(DateTime.Now.Millisecond);

            for (var i = 2; i <= 8; i++)
            {
                int numero;
                do
                {
                    numero = rnd.Next(0, 7);
                } while (numeros[numero]);

                numeros[numero] = true;

                var carta = (Carta)mesa.FindName(string.Format("carta{0}", i));
                carta.Tag = numero;
                ReposicionaCarta(carta);
            }
            posicaoMarcador = 0;
            Canvas.SetLeft(marcador, 1);
            Canvas.SetTop(marcador, 90);
        }
    }
}
