using System;
using System.Collections.Generic;
using System.Linq;

namespace CarlosRojas.Utilidades.Pupiletras
{
    /// <summary>
    /// Clase utilitaria que permite generar pupiletras (sopa de letras) a partir de un listado de palabras, un tamaño y algunos otros parámetros
    /// </summary>
    public class Generador
    {
        private int _tamano = 2;
        public int Tamano
        {
            get { return _tamano; }
            set
            {
                if (value >= 2)
                {
                    _tamano = value;
                }
            }
        }

        public List<string> Palabras { get; set; }
        public List<string> PalabrasExcluidas { get; private set; }

        private bool _sentidoDirecto = true;
        public bool SentidoDirecto
        {
            get { return _sentidoDirecto; }
            set
            {
                if (!(value == false && _sentidoInverso == false))//Valida que si el otro sentido es falso, no se le asigne falso a este sentido
                    _sentidoDirecto = value;
            }
        }
        private bool _sentidoInverso = true;
        public bool SentidoInverso
        {
            get { return _sentidoInverso; }
            set
            {
                if (!(value == false && _sentidoDirecto == false))//Valida que si el otro sentido es falso, no se le asigne falso a este sentido
                    _sentidoInverso = value;
            }
        }

        private int _reintentosMax = 0;
        public int ReintentosMax
        {
            get { return _reintentosMax; }
            set
            {
                if (value >= 0)
                {
                    _reintentosMax = value;
                }
            }
        }

        private int milisegundosMax = 100;
        private int milisegundosLimite = 5;
        private Random aleatorio = new Random(DateTime.Now.Millisecond);
        private char[,] pupiletras;

        /// <summary>
        /// Método que emplea el método generarPupiletras para generar un pupiletras y lo devuelve en una lista de caracteres
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public List<char> generarPupiletrasToList(int tipo = 1)
        {
            List<char> pupiletrasToList = new List<char>();
            pupiletras = generarPupiletras(tipo);

            if (pupiletras != null)
                for (int i = 0; i < pupiletras.GetLength(0); i++)
                {
                    for (int j = 0; j < pupiletras.GetLength(1); j++)
                    {
                        pupiletrasToList.Add(pupiletras[i, j]);
                    }
                }

            return pupiletrasToList;
        }//generarPupiletrasToList

        /// <summary>
        /// Método que genera el pupiletras y lo devuelve en una matriz
        /// </summary>
        /// <param name="tipo">Tipo 1 (valor por defecto) genera el pupiletras completo. Tipo 0 genera pupiletras sin distractores</param>
        /// <returns>Pupiletras generado (matriz de TamanoxTamano)</returns>
        public char[,] generarPupiletras(int tipo = 1)
        {
            pupiletras = null;
            if (estaValidadoPalabras())
            {
                int reintentos = 0;
                do
                {
                    PalabrasExcluidas = new List<string>();
                    generarPalabras();
                    reintentos++;
                } while (PalabrasExcluidas.Count > 0 && reintentos <= ReintentosMax);

                completarMatriz(tipo);
            }

            return pupiletras;
        }//generarPupiletras

        private void generarPalabras()
        {
            pupiletras = new char[Tamano, Tamano];
            int fila, columna;
            int[] coordenadas = new int[2];
            char sentido;
            char direccion;

            foreach (string palabra in ordenarPorLongitudDescendente(Palabras))//Ingresar las palabras a la matriz empezando por las de mayor longitud
            {
                //direccion = getDireccionAleatoria();
                //sentido = getSentidoAleatorio();

                DateTime tiempoInicial = DateTime.Now;
                DateTime tiempoFinal = DateTime.Now;

                int contador = 0;
                do
                {
                    contador++;
                    direccion = getDireccionAleatoria();
                    sentido = getSentidoAleatorio();
                    coordenadas = getCoordenadas(direccion, sentido, palabra);
                    tiempoFinal = DateTime.Now;
                } while (coordenadas == null && (tiempoFinal - tiempoInicial).Milliseconds <= milisegundosMax);

                if (coordenadas == null)
                {
                    PalabrasExcluidas.Add(palabra);
                    continue;
                }

                fila = coordenadas[0];
                columna = coordenadas[1];

                switch (direccion)
                {
                    case 'H':
                        //Sentido: -->
                        if (sentido == 'D')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                columna++;
                            }
                        }
                        //Sentido: <--
                        else if (sentido == 'I')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                columna--;
                            }
                        }
                        break;

                    case 'V':
                        //Sentido: Arriba hacia Abajo
                        if (sentido == 'D')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila++;
                            }
                        }
                        //Sentido: Abajo hacia Arriba
                        else if (sentido == 'I')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila--;
                            }
                        }
                        break;

                    case 'I':
                        //Sentido: ArribaIzquierda a AbajoDerecha
                        if (sentido == 'D')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila++;
                                columna++;
                            }
                        }
                        //Sentido: AbajoDerecha a ArribaIzquierda
                        else if (sentido == 'I')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila--;
                                columna--;
                            }
                        }
                        break;

                    case 'D':
                        //Sentido: AbajoIzquierda a ArribaDerecha
                        if (sentido == 'D')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila--;
                                columna++;
                            }
                        }
                        //Sentido: ArribaDerecha a AbajoIzquierda
                        else if (sentido == 'I')
                        {
                            foreach (char letra in palabra.ToUpper())
                            {
                                pupiletras[fila, columna] = letra;
                                fila++;
                                columna--;
                            }
                        }
                        break;

                    default://Nunca debe llegar acá
                        break;
                }
            }
        }//generarPalabras

        private bool estaValidadoPalabras()
        {
            bool esValido = true;
            if (Palabras != null && Palabras.Count <= Tamano * Tamano)
            {
                foreach (string palabra in Palabras)
                {
                    if (palabra.Length > Tamano || palabra.Length < 2)//Validar que las palabras no sean más largas que el tamaño del arreglo y no tengan menos de 2 caracteres
                        esValido = false;
                }
            }
            else
            {
                esValido = false;
            }
            return esValido;
        }//estaValidadoPalabras

        private void completarMatriz(int tipo)
        {
            if (tipo == 1)//Completa la matriz con letras mayúsculas aleatorias
            {
                for (int i = 0; i < pupiletras.GetLength(0); i++)
                {
                    for (int j = 0; j < pupiletras.GetLength(1); j++)
                    {
                        if (!char.IsLetter(pupiletras[i, j]))
                            pupiletras[i, j] = (char)aleatorio.Next(65, 91);
                    }
                }
            }
            else if (tipo == 0)//Completa la matriz con puntos (para poder identificar las palabras fácilmente)
            {
                for (int i = 0; i < pupiletras.GetLength(0); i++)
                {
                    for (int j = 0; j < pupiletras.GetLength(1); j++)
                    {
                        if (!char.IsLetter(pupiletras[i, j]))
                            pupiletras[i, j] = '.';
                    }
                }
            }
        }//completarMatriz

        private char getDireccionAleatoria()
        {
            char[] direcciones = { 'H', 'V', 'I', 'D' };
            //return 'H';
            return direcciones[aleatorio.Next(0, direcciones.Length)];
        }//getDireccionAleatoria

        private char getSentidoAleatorio()
        {
            List<char> sentidos = new List<char>();
            if (SentidoDirecto)
                sentidos.Add('D');
            if (SentidoInverso)
                sentidos.Add('I');

            return sentidos[aleatorio.Next(0, sentidos.Count)];
        }//getSentidoAleatorio

        private int[] getCoordenadas(char direccion, char sentido, string palabra)
        {
            int longitud = palabra.Length;

            int x = 0, y = 0;
            int[] coordenadas = new int[2];

            bool seguir = true;
            //long contador = 0;

            DateTime tiempoInicial = DateTime.Now;
            DateTime tiempoFinal = DateTime.Now;

            while (seguir)
            {
                //contador++;
                if ((tiempoFinal - tiempoInicial).Milliseconds >= milisegundosLimite)//Si luego de más de X milisegundos no encuentra una coordenada
                {
                    return null;
                }

                x = aleatorio.Next(0, Tamano);
                y = aleatorio.Next(0, Tamano);

                switch (direccion)
                {
                    case 'H':// -
                        if (sentido == 'D')
                        {
                            if ((x + longitud) <= Tamano && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        else if (sentido == 'I')
                        {
                            if ((x - longitud + 1) >= 0 && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        break;
                    case 'V':// |
                        if (sentido == 'D')
                        {
                            if ((y + longitud) <= Tamano && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        else if (sentido == 'I')
                        {
                            if ((y - longitud + 1) >= 0 && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        break;
                    case 'I':// \
                        if (sentido == 'D')
                        {
                            if (((x + longitud - 1) < Tamano) && ((y + longitud - 1) < Tamano) && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        else if (sentido == 'I')
                        {
                            if (((x - longitud + 1) >= 0) && ((y - longitud + 1) >= 0) && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        break;
                    case 'D':// /
                        if (sentido == 'D')
                        {
                            if (((x + longitud - 1) < Tamano) && ((y - longitud + 1) >= 0) && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        else if (sentido == 'I')
                        {
                            if (((x - longitud + 1) >= 0) && ((y + longitud - 1) < Tamano) && estaLibre(palabra, x, y, direccion, sentido))
                                seguir = false;
                        }
                        break;
                    default://Nunca debe llegar acá
                        break;
                }//switch
                tiempoFinal = DateTime.Now;
            }//while

            coordenadas[0] = y;//fila
            coordenadas[1] = x;//columna

            return coordenadas;
        }//getCoordenadas

        private bool estaLibre(string palabra, int x, int y, char direccion, char sentido)
        {
            bool seguir = true;
            switch (direccion)
            {
                case 'H':// -
                    if (sentido == 'D')
                    {
                        for (int f = y, c = x, z = 0; z < palabra.Length && seguir; c++, z++)
                        {
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                        }
                    }
                    if (sentido == 'I')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); c--, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    break;
                case 'V':// |
                    if (sentido == 'D')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); f++, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    if (sentido == 'I')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); f--, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    break;
                case 'I':// \
                    if (sentido == 'D')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); c++, f++, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    if (sentido == 'I')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); c--, f--, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    break;
                case 'D':// /
                    if (sentido == 'D')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); c++, f--, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    if (sentido == 'I')
                    {
                        for (int f = y, c = x, z = 0; (z < (palabra.Length) && seguir); c--, f++, z++)
                            if (char.IsLetter(pupiletras[f, c]) && pupiletras[f, c] != palabra[z])
                                seguir = false;
                    }
                    break;
                default://Nunca debe llegar acá
                    break;
            }//switch
            return seguir;
        }//estaLibre

        private static IEnumerable<string> ordenarPorLongitudDescendente(IEnumerable<string> palabras)
        {
            //Uso de LINQ para ordenar la colección de forma descendente y retornar una copia.
            var palabrasOrdenadas = from p in palabras
                                    orderby p.Length descending
                                    select p;
            return palabrasOrdenadas;
        }//ordenarPorLongitudDescendente

    }//class
}//namespace