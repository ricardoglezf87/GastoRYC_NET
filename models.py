class Transaccion:
    def __init__(self, fecha, cuenta, descripcion, transferir, debito, credito, saldo):
        self.fecha = fecha
        self.cuenta = cuenta
        self.descripcion = descripcion
        self.transferir = transferir
        self.debito = debito
        self.credito = credito
        self.saldo = saldo
