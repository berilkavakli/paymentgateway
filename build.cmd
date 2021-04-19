docker build -t paymentgateway -f PaymentGateway.dockerfile .
docker build -t bankservicea -f BankServiceA.dockerfile .
docker build -t bankserviceb -f BankServiceB.dockerfile .
pause