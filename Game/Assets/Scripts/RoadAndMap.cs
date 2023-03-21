using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadAndMap : MonoBehaviour
{

    //Variaveis Iniciais

    [SerializeField] private int road_size = 32;    // Declara Tamanho da estrada - valor sugerido inicialmente 32 
    [SerializeField] private int map_rows = 15;     // Declara Tamanho do mapa por linha - valor sugerido inicialmente 15
    [SerializeField] private int map_colunms = 25;  // Declara Tamanho do mapa por coluna - valor sugerido inicialmente 25
    [SerializeField] private float tileSize = 1f;   // Declara Tamanho dos tile - usar o valor 1, valores diferentes apenas para ajudar a debugar
    /* 
     * Obs1.: Dependendo de alguns valores e do tamanho do mapa pode dar possiveis bugs, porque o codigo calcula para se encaixar no mapa
            para evitar esses bugs, apenas colocar o road_size com tamanho de numero PAR, de preferencia numero multiplos de 4
     * Motivo1.: Quando é criado a Grid, é criada em um Quadrado/Retangulo, caso seja um numero IMPAR, vai faltar ou sobrar um tile, não completando (ou estourando) o array
     * Motivo2.: É preferencia de numero multiplo de 4, Se a quantidade de linha (map_rows) for grande, porque o quadrado ter a mesma quantidade de tiles dos 4 lados, 
            caso o valor das linhas sejam menor é expandido para os lados, então o numero não precisa mais ser obrigatoriamente multiplo de 4 (porém ainda precisa ser PAR)

     * Obs2.: Caso o valor da estrada seja um valor que dependo da hora que for criado ou de quando é criada a aleatoriedade for muito proxima do valor de linhas/colunas, pode quebrar a estrada e ficar estranho.
            Então é recomendado fazer testes dos tamanhos para evitar erro na versão lançada
    */

    public GameObject[] road;                       // Onde é armazenada os tiles da estrada
    public GameObject[] map;                        // Onde é armazenada os tiles do mapa (nesse caso, tudo que não seja estrada)
    public GameObject[,] road_xy;                   // Onde é armazenada todos os tiles, divido em 2 posições, para facilitar na hora de trocar posição da estrada com o mapa

    int road_rowStart;                              // Onde é armazenada qual posição vai começar a estrada em relação a linha
    int road_rowEnd;                                // Onde é armazenada qual posição vai termina a estrada em relação a linha
    int road_colunmsStart;                          // Onde é armazenada qual posição vai começar a estrada em relação a coluna
    int road_colunmsEnd;                            // Onde é armazenada qual posição vai termina a estrada em relação a coluna

    private int road_1;                             // Onde é armazenada quantos tiles vão ter por linha 
    private int road_2;                             // Onde é armazenada quantos tiles vão ter por coluna


    // Quando a cena abre o Start() abre 
    void Start() {
        RoadSize_Organizar();                                               // Corrigi bug e a partir de agora pode ser colocado qualquer valor no road_size    

        road = new GameObject[road_size];                                   // Declara que o tamanho da estrada vai ter o tamanho que foi definido no road_size
        road_xy = new GameObject[map_rows, map_colunms];                    // Declara que o tamanho da total do mapa vai ter o tamanho que foi definido nas variaveis map_rows, map_colunms - sendo numero de linha e coluna
        map = new GameObject[(map_rows * map_colunms) - road_size];         // Declara que o tamanho da estrada vai ter o tamanho que foi definido no road_size
        GenerateGrid();                                                     // Chama a função que Gera o mapa criando um grid 
        Road_Suffle();                                                      // Chama a função que Deixa o mapa aleatorio

        criarPlayer();
    }
    private void RoadSize_Organizar() {
        /*
         Enquanto não for multiplo de 4, adiciona 1 para o valor final ser multiplo de 4
         */
        while(road_size % 4 != 0) {
            road_size++;
        }

        /*
         Verifica se o tamanho tamanho da estrada divido por 4 é maior ou menor que o tamanho da linha menos por tres, enquanto estiver nessa condição adiciona uma linha 
         */
        while (road_size / 4 >= map_rows - 3) {
            map_rows++;
        }
    }
    public void GenerateGrid() {
        GameObject map_ref = (GameObject)Instantiate(Resources.Load("road_0"));     // Busca o GameObject do mapa na pasta Asset/Resource criando uma referencia
        GameObject road_ref = (GameObject)Instantiate(Resources.Load("road_1"));    // Busca o GameObject da estrada na pasta Asset/Resource criando uma referencia

        /* -- start if -- */
        /*
            Esse if verifica se o tamanho da estrada dividido por 4 sobra linha, para dividir quantos tiles vão ter na linha e na coluna
            Se a quantidade de linha sobrar for menor ou igual a 3, vão ter a quantidade de linhas para ter pelo menos uma folga de 3 linha do topo e da base do mapa
            Se o valor estiver ok, tanto a linha quanto a coluna vão ter o mesmo valor de tiles

            * Obs.: O valor é divido por 4, porque leva em conta 2 linhas e 2 colunas
        */
        if (road_size/4 >= map_rows - 3) {
            road_1 = map_rows - 3;
            road_2 = (road_size - (road_1 * 2))/2;
        } else {
            road_1 = road_size / 4;
            road_2 = road_size / 4;
        }
        /* -- end if -- */

        road_rowStart = (map_rows - (road_1 + 1)) / 2;          // Levando em conta a quantidade de linhas que tem menos uma folga para cima do tamanha do mapa, da o valor onde começa as linhas
        road_rowEnd = road_rowStart + (road_1 + 1);             // Levando em conta a quantidade de linhas que tem menos uma folga para baixo do tamanha do mapa, da o valor onde termina as linhas
        road_colunmsStart = (map_colunms - (road_2 + 1)) / 2;   // Levando em conta a quantidade de colunas que tem menos uma folga para esquerda do tamanha do mapa, da o valor onde começa as colunas
        road_colunmsEnd = road_colunmsStart + (road_2 + 1);     // Levando em conta a quantidade de colunas que tem menos uma folga para direita do tamanha do mapa, da o valor onde termina as colunas

        int road_int = 0;   // Valor para calcular qual o index (no array) da estrada que estara no for
        int map_int = 0;    // Valor para calcular qual o index (no array) do mapa que estara no for

        int road_x = 0;     // Valor para calcular qual o index (no array) (apenas do x) de todo o mapa/estrada
        int road_y = 0;     // Valor para calcular qual o index (no array) (apenas do y) de todo o mapa/estrada


        for (int row = map_rows; row > 0; row--) {              // for rodando por todas as linhas
            for (int col = 0; col < map_colunms; col++) {       // for rodando por todas as colunas

                GameObject tile;    //Cria um objeto 'tile'


                /* start if */
                /*
                    Esse if verifica se o tile esta passando pelas linhas e pelas colunas que foram definidas (Que começam e que terminam)
                    Se estiver entre o começo e o final da linha E for o começo OU o final da coluna
                        OU o começo e o final da coluna E o começo OU final da linha
                    O tile é definido como estrada, é salva no array road e é adicionado 1 ao road_int, para caso haja mais uma estrada, ser salva no  index correto

                    Caso contrario
                    O tile é definido como mapa, é salva no array map e é adicionado 1 ao map_int, para caso haja mais uma mapa, ser salva no  index correto
                */
                if (((row > road_rowStart && row <= road_rowEnd) &&               
                   (col == road_colunmsStart || col == road_colunmsEnd - 1)) ||     
                   ((row == road_rowStart + 1 || row == road_rowEnd) &&
                   (col >= road_colunmsStart && col < road_colunmsEnd))) {  
                    tile = (GameObject)Instantiate(road_ref,transform);
                    road[road_int] = tile;
                    road_int++;
                } else {
                    tile = (GameObject)Instantiate(map_ref, transform);
                    map[map_int] = tile;
                    map_int++;
                }
                /* -- end if -- */

                /* -- start centralizar mapa  -- */
                /*
                    Estas 3 linhas a seguir, defini a posição na cena onde vai ser criado o mapa
                    Está parte existe para ao criar o mapa por um todo, ele sempre esteja no centro da cena

                    É criado uma posição X e Y e feito um calculo 
                        sendo que X é o resultado da posição da coluna vezes o tamanho do tile menos o tamanho da quantidade de colunas dividido por 2 vezes o tamanho do tile
                        sendo que Y é o resultado da posição da linhas vezes o tamanho do tile menos o tamanho da quantidade de linhas dividido por 2 vezes o tamanho do tile
                    O resultado desses 2 valores é a posição que o tile ficara no mapa
                 */
                float posX = (col * tileSize) - (map_colunms / 2 * tileSize);
                float posY = (row * tileSize) - (map_rows / 2 * tileSize);
                tile.transform.position = new Vector2(posX, posY);
                /* -- end centralizar mapa  -- */

                /* -- Definir posição do Array do mapa todo --  */
                /*
                    Todos os tiles são salvos no road_xy, 
                    Esse array foi divido em 2 vetores, porque isso ajuda a manipular a posição de cada tile, sabendo em qual linha e coluna aquele bloco esta
                    Essa parte do codigo é apenas para o index passar por cada coluna e se o valor chegar na coluna do ultimo tile, ela pula para proxima linha repetindo o processo                    
                 */
                road_xy[road_x, road_y] = tile;
                road_y ++;
                if(road_y >= map_colunms) {
                    road_y = 0;
                    road_x++;
                }
                /* -- Definir posição do Array do mapa --  */
            }
        }
        Destroy(road_ref);      // exclui a referencia do item da estrada porque já foi usado para criar os tiles
        Destroy(map_ref);       // exclui a referencia do item do mapa porque já foi usado para criar os tiles

        Road_Organizar();       // Chama a função que organiza os Tiles da estrada
    }

    public void Road_Organizar() {
        /*
              A função de organizar a estrada existe, porque o mapa, por um todo, é criado da esquerda para direita linha por linha, 
            porém para trocar os tiles da estrada de uma forma que a estrada se feche no mesmo lugar que ela começa, tem que ser feita de forma sequencial,
            no caso, quando a linha acaba, o proximo tile tem que ser o seguinte que estiver ligado a ele.

              Essa função funciona da seguinte forma, é calculado quantos blocos vão ter em cada linha/coluna
           * Exemplo.: Se a estrada tiver 32 tiles, no topo e na base vão ter 9 tiles e em cada lado vão ter 6, porque a logica que foi feita é considerada por linha
             
              Quando sabemos quantos tiles vão ter em cada lado, separados o index de cada lado e dos 2 lados juntos(é importante ter dos 2 lados juntos, para conseguir separar os lados)
              * É separado tambem o index geral da estrada

              É criado um for, passando por toda a road (estrada) e é verificado 3 situações
              1) SE o index do for, estiver no tamanho da linha
              2) SE o index do for, estiver maior que o tamanho da linha e menor que o tamanho da linha vezes 2 vezes o tamanho da coluna menos 1 (para não considerar a coluna de baixo)
              3) SE o index do for, for diferente da 2 opções a cima

              No primeiro caso é salvo no topo
              No segundo caso é verificado 2 fatores
                SE o numero do index da coluna total for par, é salvo na esquerda
                SE o numero do index da coluna total for impar, é salvo na direita
              No terceiro caso é salvo na base
              
              * Obs1.: Cada vez que passa por cada condição adiciona 1 ao index da sua respectiva direção
              * Obs2.: É salvo a coluna 2 vezes, sendo o geral e cada lado, porque como é feito em 2 linhas, essa foi a forma encontrada de separar dos 2 lados, porque como o mapa é criado
                     da esquerda para direita, o valor da esquerda sempre sera par e direita impar

              Depois de separado, é criado 4 for, para ser recolocado os itens na ordem correta para futuramente deixar o mapa de forma aleatoria
              O primeiro for adiciona o topo
              O segundo for adiciona a direita
              O terceiro for adiciona a base
              O quarto for adiciona a esquerda

              * Obs1.: No caso do terceiro e o quarto codigo é trocado a ordem que vai ser adicionada, no caso os itens são adicionado do ultimo para o primeiro, porque as linhas são criadas de cima para baixo, esquerda para direita
              * Obs2.: No caso desses 2 lados que são feitos ' de trás para frente', é criado uma variavel para salvar o valor total, pois o for da conflito com o calculo do valor
              * Obs2.: Para controlar o index da road por passar por 4 for, é usado o index geral da estrada (road__index)
         */

        GameObject[] road_top = new GameObject[road_1 + 1];
        GameObject[] road_right = new GameObject[road_2 - 1];
        GameObject[] road_bottom = new GameObject[road_1 + 1];
        GameObject[] road_left = new GameObject[road_2 - 1];


        int road_top_index = 0;
        int road_right_index = 0;
        int road_bottom_index = 0;
        int road_left_index = 0;
        int road_colunm_index = 0;

        int road__index = 0;

        for (int i = 0; i < road.Length; i++) {
            if (i <= road_1) {
                //primeira linha
                road_top[road_top_index] = road[i];
                road_top_index++;
            } else if (i > road_1 && i < (road_1 + ((road_2) * 2)) - 1) {
                //linhas do meio
                if (road_colunm_index % 2 == 0) {
                    road_left[road_left_index] = road[i];
                    road_left_index++;
                } else {
                    road_right[road_right_index] = road[i];
                    road_right_index++;
                }
                road_colunm_index++;
            } else {
                //ultimas linhas
                road_bottom[road_bottom_index] = road[i];
                road_bottom_index++;
            }
        }


        for (int i = 0; i < road_top.Length; i++) {
            road[road__index] = road_top[i];
            road__index++;
        }
        for (int i = 0; i < road_right.Length; i++) {
            road[road__index] = road_right[i];
            road__index++;
        }
        int inverter_for_road = road_bottom.Length - 1;
        for (int i = inverter_for_road; i >= 0; i--) {
            road[road__index] = road_bottom[i];
            road__index++;
        }
        inverter_for_road = road_left.Length - 1;
        for (int i = inverter_for_road; i >= 0; i--) {
            road[road__index] = road_left[i];
            road__index++;
        }

    }

    public void Road_Suffle() {
        /*
            A função de 'aleatoriezar' a estrada existe para quando é chamada criar um caminho diferente a cada vez

            É criado um array que salva a posição inicial dos tiles da estrada. // * OBS.: esse array é importante para as alterações não serem feitas diretamente nos tiles e poder ser manipulado mais para frente
            Declara uma variavel de segurança(pode), que defini que caso quando um tile é mudado de valor, o proximo não ser mudado, para não ficar 'gruadado' varios tiles // * OBS.: foi feito diversos testes e o valor ideial é 3
            
            //----------------------------------------------------------------------------------------------------------//
                É criado um for que salva as posições nos tiles, no array que foi criado
                //-----------------------------------------------------------------------------------------------------------//
                    É criado o for que é feita o calculo da aleatoriedade, nesse for, é criado uma variavel que toda vez que é chamada é criada um numero aleatorio, em seguida, salva  valor x e y da respectiva posição do array road_position
                    O calculo é feito a partir do segundo tile, porque o primeiro será usado como referencia para o codigo e a partir dai é criado um valor que salva a posição do tile anterior
               
                    O Resumo da conta que é feita no codigo é:
                      Nas posições:
                        O valor da primeira linha mais 1
                        O valor da primeira linha mais a primeira coluna menos 1
                        O valor da primeira linha mais a primeira coluna mais 1
                        O valor da primeira linha mais a primeira coluna mais a metade da segunda linha
                        O valor da primeira linha mais a primeira coluna mais a segunda linha mais 1
                        O resto dos tiles que faltam (no caso as 2 linhas e as 2 colunas)
                
                    É verificado SE 50% da chance puder mudar, ele troca de posição
                //-----------------------------------------------------------------------------------------------------------//
                Diminui o valor da variavel que ve se pode mudar, porque quando chega a 0, é liberado ( e caso seja mudado, volta para 3) // * OBS.: É criado uma trava, para não ser menor que 0
                É substituido o valor das posições x e y do array que salvava o valor inicial dos tiles
            //----------------------------------------------------------------------------------------------------------//
            
            O ultimo for, em resumo, cria 2 valores temporarios:
              verifica qual tile é que tem a mesma posição do item do array que salva a posicao
              verifica qual tile represtenta o tile da estrada e salva a posicao inicial

            Logo em seguida é trocado os valores da estrada, com o tile do mapa que estiver com a mesma posição
         
         */
        Vector3[] road_position = new Vector3[road.Length];
        int podeMudar = 3;

        for (int i = 0; i < road.Length; i++) {
            road_position[i] = road[i].transform.position;
        }

        for (int i = 0; i < road_position.Length; i++) {
            float randomNumber = Random.Range(0, 100);

            float road_x = road_position[i].x;
            float road_y = road_position[i].y;
            if (i > 1) {
                float road_xOld = road_position[i - 1].x;
                float road_yOld = road_position[i - 1].y;
                /* ---------------------------- */
                if (i < road_1 + 1) {
                    if (randomNumber >= 50 && podeMudar == 0) {
                        road_y = road_yOld - 1;
                        road_x = road_xOld;
                        podeMudar = 3;
                    } else {
                        road_y = road_yOld;
                        road_x = road_xOld + 1;
                    }
                } else if (i < (road_1 + road_2) - 1) {
                    if (randomNumber >= 50 && podeMudar == 0) {
                        road_y = road_yOld;
                        road_x = road_xOld + 1;
                        podeMudar = 3;
                    } else {
                        road_y = road_yOld - 1;
                        road_x = road_xOld;
                    }
                } else if (i < (road_1 + road_2) + 1) {
                    road_y = road_yOld - 1;
                    road_x = road_xOld; ;
                } else if (i < (road_1 + road_2) + (road_1/2)) {
                    road_y = road_yOld;
                    road_x = road_xOld - 1;
                } else if (i < (road_1 + road_2 + road_1) + 1) {
                    if (road_xOld - 1 >= road[0].transform.position.x) {
                        if (randomNumber >= 50 && podeMudar == 0) {
                            road_y = road_yOld + 1;
                            road_x = road_xOld;
                            podeMudar = 3;
                        } else {
                            road_y = road_yOld;
                            road_x = road_xOld - 1;
                        }
                    } else {
                        road_y = road_yOld + 1;
                        road_x = road_xOld;
                    }
                } else {
                    if (road_xOld - 1 >= road_x) {
                        road_y = road_yOld;
                        road_x = road_xOld - 1;
                    } else {
                        road_y = road_yOld + 1;
                        road_x = road_xOld;
                    }
                }
                /* ---------------------------- */
            }
            podeMudar--;
            if (podeMudar <= 0) podeMudar = 0;
            road_position[i].x = road_x;
            road_position[i].y = road_y;
        }


        for (int i = 0; i < road_position.Length; i++) {
            int _xx = 0;
            int _yy = 0;
            for (int j = 0; j < road_xy.Length; j++) {
                if(road_position[i] == road_xy[_xx, _yy].transform.position) {
                    Vector3 tempPos_1 = road_xy[_xx, _yy].transform.position;
                    Vector3 tempPos_2 = road[i].transform.position;

                    road_xy[_xx, _yy].transform.position = tempPos_2;
                    road[i].transform.position = tempPos_1;

                }
                _yy++;
                if (_yy >= map_colunms) {
                    _yy = 0;
                    _xx++;
                }
            }
        }
    }

    void criarPlayer() {
        GameObject player_ref = (GameObject)Instantiate(Resources.Load("Player"));
        GameObject tile;

        tile = (GameObject)Instantiate(player_ref, transform);

        tile.transform.position = road[1].transform.position;
        Destroy(player_ref);
    }


}
