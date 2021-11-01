# Validações de CNPJ
Um algorítimos simples de validação de CNPJ, cujo o intuito é mostrar como o Garbage Collector pode afetar a performance do algorítimo.

## Explicação
O número que compõe o CNPJ é composto por três segmentos de algarismos, sendo o primeiro o número da inscrição propriamente dito, o segundo (após a barra) o número de filiais e o terceiro representados pelos últimos dois valores que são os dígitos verificadores.
Oficialmente o cálculo do número do CNPJ prevê também a verificação do oitavo dígito, mas algumas empresas possuem números que ao serem validados segundo esse critério são considerados inválidos.
Por isso o mais seguro é você fazer a validação dos dígitos verificadores, pois assim nenhum número será inválido e sua rotina está protegida da mesma forma, já que a regra é única e funciona com qualquer CNPJ válido.

## Algorítimo comum
No algorítimo que praticamente é o mais comum a ser adotado, podemos verificar que:

Primeiramente é feito uma tratativa na string eliminando espaços em branco e removendo os caracteres especiais. Porém, para cada método chamado, uma nova string é alocada na memória HEAP, gerando um trabalho futuro para o nosso GC, logo, ocasionando suspensões na execução do nosso programa.

Depois, podemos verificar que é alocado dois arrays de inteiro em toda execução de validação. Esses números são contantes e nunca mudam, então não faz sentido em toda execução do algorítimo, eu alocar esses arrays na memória. Como exemplo, se o nosso programa está executando este fluxo 2 milhões de vezes, então será alocado em memória 4 milhões de arrays, e como este é um array de inteiro, cada posição são 4 bytes alocados na memória onde o primeiro array em sua totalidade vai possuir 48 bytes de tamanho e o segundo 52 bytes por execução.

Podemos verificar tmbém, que estamos fazendo um loop em cada char da nossa string, convertendo o char para string e parseando a string para inteiro para assim multiplicar pelo devido multiplicador. Quando eu dou o ToString, podemos lembrar que string estão sempre na memória HEAP, logo a cada ToString eu aloco um elemento na HEAP gerando assim, pressão sobre o GC.

## Algorítimo aprimorado
No algorítimo em que nos importamos em aprimorar as chamadas do GC, observe que levamos os arrays multiplicadores para fora da execução do método, tornando-os constantes.

Alocamos também, um array na memória STACK. Fizemos isso, pois como não recebemos por parâmetro, essa função não é recursiva, então eu não vou estourar a stack fazendo esta alocação. Então não tem porque eu alocar um novo array na HEAP.

A conversão de dígito em char, podemos utilizar um macete. Quando temos por exemplo o char '9' e queremos convertê-lo para inteiro, podemos utilizar a subtração com o char '0'. Veja como isso funciona: 

CHAR | VALUE 
--- | ---
'9' | 57
'0' | 48

Então se fizemos '9' - '0', teremos 57 - 48, que dá o próprio valor do char em inteiro, convertendo assim nosso char em interio sem nenhuma alocação desnecessária.

O restante foi somente aprimoramento do algorítimo.

## Execuções
Utilizando a minha máquina como base, obtive os seguintes resultados:

#### Comum:
![alt text](https://github.com/Farllon/cnpj-validations-gc/blob/main/common.png?raw=true)

#### Aprimorado:
![alt text](https://github.com/Farllon/cnpj-validations-gc/blob/main/fast.png?raw=true)

Em algumas execuções, o algorítimo aprimorado foi 10x mais eficaz do que o comum.
