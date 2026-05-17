# Payments API - Fase 4

Worker .NET 8 responsavel por processar solicitacoes de compra recebidas via RabbitMQ dentro do ecossistema Fase 4. O projeto consome eventos de pagamento, aplica uma regra simples de aprovacao/rejeicao e publica uma notificacao para a fila de emails.

## Visao geral

A Payments API e um worker, nao uma Web API HTTP. Sua responsabilidade principal e:

- Consumir mensagens `PurchaseRequestedEvent` da fila `payment-queue`.
- Ler eventos publicados no exchange RabbitMQ `fiap.events`.
- Processar a regra de pagamento.
- Publicar `EmailNotificationEvent` na fila/routing key `notification-queue`.
- Gravar logs em console e em arquivos rotativos.

## Fluxo

1. Games API publica uma solicitacao de compra em `payment-queue`.
2. Payments API consome o evento `PurchaseRequestedEvent`.
3. O worker aplica a regra de pagamento:
   - `Amount >= 100`: aprovado.
   - `Amount < 100`: rejeitado.
4. Payments API publica uma mensagem `EmailNotificationEvent` em `notification-queue`.
5. Notifications API consome a notificacao e processa o envio.

## Arquitetura

- .NET 8 Worker Service.
- MassTransit com RabbitMQ.
- Contratos compartilhados em `src/Shared.Contracts`.
- Infraestrutura de consumo/publicacao em `src/PaymentsAPI.Infrastructure`.
- Testes em `src/PaymentsAPI.Infrastructure.Tests`.
- Dockerfile e Docker Compose em `src`.
- Kubernetes namespace `fase4`.
- Imagem Docker `adinteltidev/fase4-payments-api:latest`.

## Contrato de entrada

```csharp
public record PurchaseRequestedEvent
{
    public string EventType => "PURCHASE_REQUESTED";
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;
    public decimal GameValue { get; init; }
    public string GameName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime RequestedAt { get; init; }
}
```

## Contrato de saida

```csharp
public record EmailNotificationEvent
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public string? Sender { get; init; }
}
```

Mensagem publicada apos o processamento:

```text
Title: Compra processada
Subtitle: Pagamento Aceito|Rejeitado
Body: Seu pagamento foi Aceito|Rejeitado, em caso de duvidas entre em contato com nosso suporte
Recipient: payments@fcg,com.br
```

## Configuracoes principais

Variaveis esperadas pelo worker:

- `RabbitMq__Host`
- `RabbitMq__Port`
- `RabbitMq__Username`
- `RabbitMq__Password`
- `RabbitMq__VirtualHost`
- `RabbitMq__ExchangeName`
- `RabbitMq__PaymentQueueName`
- `RabbitMq__NotificationQueueName`
- `MT_LICENSE`

Valores padrao usados no projeto:

- RabbitMQ host local: `localhost`
- RabbitMQ porta AMQP: `5672`
- RabbitMQ Management: `15672`
- Virtual host: `fiap`
- Exchange: `fiap.events`
- Fila/routing key de pagamentos: `payment-queue`
- Fila/routing key de notificacoes: `notification-queue`

## Execucao local com .NET

```powershell
dotnet restore PaymentsAPI.sln
dotnet run --project src/PaymentsAPI/PaymentsAPI.Worker.csproj
```

Para rodar desse modo, o RabbitMQ precisa estar disponivel e as variaveis `RabbitMq__*` devem apontar para ele.

## Execucao local com Docker Compose

```powershell
cd src
docker compose up --build
```

Servicos locais:

- Payments worker: container `fiap-payments-worker`
- RabbitMQ AMQP: `localhost:5672`
- RabbitMQ Management: `http://localhost:15672`

O Compose usa recursos compartilhados:

| Recurso | Nome |
| --- | --- |
| Compose project | `fase4-paymentsapi` |
| Container RabbitMQ | `fiap-rabbitmq` |
| Network | `fiap-ms-network` |
| Volume | `fiap-rabbitmq-data` |
| Virtual host | `fiap` |
| Exchange | `fiap.events` |
| Payment queue/routing key | `payment-queue` |
| Notification queue/routing key | `notification-queue` |

## Execucao local com Kubernetes

```powershell
$env:RABBITMQ_USERNAME="admin"
$env:RABBITMQ_PASSWORD="admin123"
$env:RABBITMQ_VHOST="fiap"
.\deployLocal.ps1
```

O script aplica:

- `k8s/namespace.yml`
- `k8s/rabbitmq-service.yml`
- `k8s/rabbitmq-deployment.yml`
- `k8s/payments-configmap.yml`
- `k8s/payments-worker-deployment.yml`

Para verificar:

```powershell
kubectl rollout status deployment/payments-worker -n fase4
kubectl get pods -n fase4
```

## Deploy no EKS

```powershell
$env:RABBITMQ_USERNAME="..."
$env:RABBITMQ_PASSWORD="..."
$env:RABBITMQ_VHOST="fiap"
.\deployEks.ps1 -ClusterName Fcg-Fase4 -Region us-east-1
```

O script conecta no cluster informado, cria/atualiza o secret `rabbitmq-secrets` e aplica os manifests Kubernetes no namespace `fase4`.

## Testes

```powershell
dotnet test PaymentsAPI.sln
```

## Stack

- .NET 8
- Worker Service
- MassTransit
- RabbitMQ
- Serilog
- Docker
- Kubernetes
- Amazon EKS
- xUnit
- Moq
