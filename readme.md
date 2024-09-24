# Teste C#

A API funciona e já está configurada com todos as chamadas necessarias, modelos e responses.

Sao duas apis: uma que busca informacoes de lat e lng a partir de um endereço e outra que usa essas informações para buscar o tempo naquela região.

O que precisa ser feito:
- Uar o MediatR para executar os handlers para cada endpoint
  - serão 2 handlers, basicamente, 1 para busca da lat e lng e outro para busca das informacoes de clima/tempo
- Adicionar uma política de retry em caso de erro na chamada
- Já que o clima n muda muito rapidamente, adicionar um cache em memória para o clima/tempo
- Adicionar pelo menos um teste unitário para o handler de do clima/tempo