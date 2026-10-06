# Pacote de DLLs para Integração com a AfterSale

## O que é o Troca Fácil?
O **Troque Fácil**, desenvolvido pela **Aftersale**, é uma plataforma de automação de pós-venda voltada para lojas virtuais (e-commerce).

O objetivo é simplificar e digitalizar o processo de trocas, devoluções e logística reversa, substituindo a burocracia do atendimento tradicional por um sistema de autoatendimento.

## Como Funciona
* **Autoatendimento**: O próprio consumidor entra na plataforma da loja, seleciona o produto que deseja trocar ou devolver e escolhe o motivo, tudo de forma autônoma.

* **Logística Reversa**: O sistema gera automaticamente as etiquetas de envio pelos Correios ou transportadoras parceiras.

* **Vale-troca ou Reembolso**: Facilita a emissão de vales-compras para que o cliente escolha outro item na loja, auxiliando o e-commerce a reter a venda.

## Principais Benefícios

* **Para a Loja**: Reduz a sobrecarga da equipe de atendimento (SAC), automatiza a geração de etiquetas e fornece relatórios de desempenho e comportamento do consumidor.

* **Para o Cliente**: Oferece uma experiência mais transparente e rápida, o que aumenta a confiança e a chance de recompra.

Para acessar o painel administrativo ou conhecer mais sobre a solução, você pode visitar o [**Portal Aftersale**](https://admin.troquefacil.com.br/login) ou conferir a integração em plataformas parceiras através da [**Central de Ajuda - Olist**](https://ajuda.olist.com/ecommerce-pedidos/ecommerce-integracao-aftersale-troque-facil).

## Sobre o Pacote
O pacote de integrações é desenvolvido em .NET 10 e arquiteturado com clean code a documentação utilizada para o desenvolvimento das integrações foi a encontrada em [After Sale API](https://app.swaggerhub.com/apis/send42/public-api/1.0.6#/)

## Disclaimer
A documentação encontrada no link acima encontra-se desatualizada, com o acesso a um Bearer Token eu efetuei uma request valida em cada um dos end-point e obtive uma resposta com alguns campos diferentes dos citados na documentação.

Com isso em mente, efetuei um de/para do json oferecido pela documentação e do json retornado pela API, os capos que não foram encontrados em ambos os jsons nao foram contemplados, pois não pude afirmar de que tipo são. 

Estarei disponibilizando o jsons obtidos para a análise no própio json explicarei cada descisão tomada em relação as propriedades

### End-Points
#### **Me**
Infos about your ecommerce
* GET /v3/api/me<br>
    Return infos about ecommerce

#### **Reverses**
Operations available to reverses

* **GET** /v3/api/reverses/status<br>
    Returns a list of possibles satuss of a reverse

* **GET** /v3/api/transportations<br>
    Returns a list of possibles transportations of a reverse

* **GET** /v3/api/reverses<br>
    Returns all reverses

* **GET** /v3/api/reverses/{id}<br>
    Return one especific reverse

* **POST** /v3/api/reverses/{id}/approve<br>
    Approve products by reverse ID

* **PUT** /v3/api/reverses/{id}/receive-products<br>
    Receive products by reverse ID

* **PUT** /v3/api/reverses/{id}/products/returned-invoice<br>
    Update recived invoice in reverse products. Need pass the products id and the returned invoice

* **GET** /v3/api/reverses/{id}/get-courier-attributes<br>
    Get courier data fields needed to process the reverse

* **POST** /v3/api/reverses/{id}/invoice<br>
    Set reverse courier data and process/generate code

#### **Refunds**
Operations available to refunds

* **GET** /v3/api/refunds/status<br>
    Returns the possible status of a refund

* **GET** /v3/api/refunds/actions<br>
    Returns the possible actions of a reverse

* **GET** /v3/api/refunds/banks<br>
    Returns the possible banks of a refund

* **GET** /v3/api/refunds/types<br>
    Returns the possible types of a refund

* **GET** /v3/api/refunds<br>
    Returns all refunds

* **POST** /v3/api/refunds/{id}/approve<br>
    Approve reverse

* **POST** /v3/api/refunds/{id}/cancel<br>
    Cancel reverse

* **POST** /v3/api/refunds/{id}/pay<br>
    Pay refund

* **POST** /v3/api/refunds/{id}/change-total-amount<br>
    Change total amount of refund

* **GET** /v3/api/refunds/{id}<br>
    Get a refund