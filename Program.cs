using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Plinko
{
    class Program
    {
        static string filas= "";
        static string columnas = "";
        static string pijasFaltantes="";
        static string final= "";
        static int _filas, _columnas, _final;

        static string[] posicionesString;

        static void Main(string[] args)
        {
            Bienvenida();  
            EscogerMejorLugar();
            Console.ReadKey();          
        }

        static void Bienvenida()
        {
            Console.WriteLine("Vamos a jugar Plinko!");
            Console.WriteLine("Escoja el número de Filas");
            filas = Console.ReadLine();
            Console.WriteLine("Escoja el número de Columnas");
            columnas = Console.ReadLine();
            Console.WriteLine("Escoja la posición de las pijas faltantes, separandolas con un ; (max 3)");
            Console.WriteLine("2,1;3,3;4,7");
            pijasFaltantes = Console.ReadLine();            
            Console.WriteLine("Escoja el lugar específico donde desea que caiga la pelota.");
            final = Console.ReadLine();
            Console.WriteLine();

            if (!ChecarPosicionPijasFaltantesYFinal(pijasFaltantes, filas, columnas, final))
            {
                Console.WriteLine("Las posiciones no se encuentran en el tablero!");
                Bienvenida();                
            } 
        }

        static bool ChecarPosicionPijasFaltantesYFinal(string pijasFaltantes, string filas, string columnas, string final)
        {            
            Int32.TryParse(filas, out _filas);
            Int32.TryParse(columnas, out _columnas);
            Int32.TryParse(final, out _final);
            posicionesString= pijasFaltantes.Split(';');

            if(posicionesString.Length > 3) //que no se exceda de 3 pijas seleccionadas
            {
                return false;
            }            

            for (int i = 0; i < posicionesString.Length; i++) 
            {
                int a,b;
                string[] helper=posicionesString[i].Split(',');
                Int32.TryParse(helper[0], out a);
                Int32.TryParse(helper[1], out b);
                if(a>_filas || b > _columnas)
                {
                    return false;
                }                          
            }

            if (_final > _columnas) //la posicion final no puede ser mayor al numero de columnas
            {
                return false;                
            }

            if(_filas%2==0 & _final%2!=0) //como empieza en cero debe ser si la fila es par final par
            {
                return false;
            }

            if(_filas%2!=0 & _final%2==0) //filas impar el final debe caer en lugar impar
            {
                return false;
            }

            return true;
        }

        static void EscogerMejorLugar()
        {          
            double mejorProb = 1;
            int[,] posicionPelota = new int[0,0];
            int inicioPruebaColumna =0;

            int _final ;
            Int32.TryParse(final, out _final);

            Tablero _tablero = new Tablero(_filas, _columnas, posicionesString);

            Console.WriteLine("Aqui esta el tablero:");

            for (int i = 0; i < _tablero.Matriz.GetLength(0); i++)
            {
                for (int j = 0; j < _tablero.Matriz.GetLength(1); j++)
                {
                     Console.Write("{0}"+ "\t",_tablero.Matriz[i,j]);                    
                }
                Console.WriteLine();                         
            }    

            //deberia ser una distribucion binomial, pero como afectan las pijas sacadas?
            //podriamos hacer ciclos probando todas las regiones donde pueden entrar una pelota, pero creo se puede ahorrar tiempo usando solo los cercanos. 
            int finalPruebaColumna=0;

            if(_final<(_tablero.Matriz.GetLength(1))/2){
                inicioPruebaColumna = 0;
                finalPruebaColumna = (int)Math.Floor((decimal)_tablero.Matriz.GetLength(1)/2);
            }
            else{
                inicioPruebaColumna = (int)Math.Floor((decimal)_tablero.Matriz.GetLength(1)/2);
                finalPruebaColumna = _tablero.Matriz.GetLength(1);
            }

            Dictionary<int,double> dict = new Dictionary<int,double>();
            #region old

            //Random rnd = new Random();

            //ciclos de movimiento de pelota

            // for (int i = 0; i < _tablero.Matriz.GetLength(0); i++)
            // {                
            //     int numeroRand = rnd.Next(0,10);
            //     for (int j = inicioPruebaColumna; j < _tablero.Matriz.GetLength(1); j++)
            //     {                  
            //         if(j==0){
            //             mejorProb = mejorProb;
            //             i++;
            //         }
            //         if(j==1){
            //             mejorProb=mejorProb * probabilidadPijaNormal;
            //             i++;
            //             if(5 >= numeroRand)
            //             {
            //                 j--;
            //                 if (j<0){j=0;}
            //             }
            //             else
            //             {
            //                 j++;
            //                 if(j>_tablero.Matriz.GetLength(1)){
            //                     j=_tablero.Matriz.GetLength(1); // -1?
            //                 }
            //             }
            //         }                    
            //     } 
            //     if(i==0){
            //           dict.Add(i,mejorProb);
            //     } 
            // }
            #endregion

            //solo vamos a contar los lugares cercanos donde existe menor cantidad de pijas, nos vamos a ir todas las y por x
            

            for (int i = inicioPruebaColumna; i < finalPruebaColumna; i++)
            {                  
                for (int j = 0; j < _tablero.Matriz.GetLength(0); j++)
                {
                    if(j==0 && _tablero.Matriz[j,i] ==1){
                        break;
                    }

                    if (_tablero.Matriz[j,i] == 1)
                    {
                        mejorProb = mejorProb*0.5;
                       
                    }                    
                }  
                dict.TryAdd(i,mejorProb);     
                mejorProb=1;         
            }

            var min = dict.Max(x => x.Value);      
            var resultadoKey = dict.Where(x => x.Value == min).Select(x =>x.Key).First();      


            Console.WriteLine("Esta es la mejor casilla: {0}",resultadoKey);
        }
    }
    public class Tablero
    {
        public int _filas {get;set;}
        public int _columnas {get;set;}  
        public int[,] Matriz {get;set;}  
        string[] _posicionesString {get;set;}   

        public Tablero(int filas, int columas, string[] posicionesString)
        {
            _filas=filas;
            _columnas=columas;   
            Matriz = new int[_filas,_columnas];
            _posicionesString=posicionesString;
            LlenarMatriz(_filas,_columnas);         
        }

        void LlenarMatriz(int filas, int columnas)
        {
            //Llenamos el tablero
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if(i%2==0 & j%2!=0)
                    {
                        Matriz[i,j] = 0;
                    }
                    else if(i%2!=0 & j%2==0)
                    {
                        Matriz[i,j] = 0;   
                    }
                    else
                    {
                        Matriz[i,j] = 1;                        
                    }                                         
                }
            }

            //colocamos los huecos de pijas
            foreach (var item in _posicionesString)
            {
                int a,b;
                var holder = item.Split(',');
                
                Int32.TryParse(holder[0],out a);
                Int32.TryParse(holder[1],out b);
                
                Matriz[a,b] = 0;                
            }
        }
    }
}
