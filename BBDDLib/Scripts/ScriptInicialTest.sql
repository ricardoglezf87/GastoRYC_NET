INSERT INTO "accountsTypes" (description) VALUES
	 ('Efectivo'),
	 ('Banco'),
	 ('Tarjetas'),
	 ('Inversiones'),
	 ('Prestamos'),
	 ('Inmovilizados'),
	 ('Ahorros');

INSERT INTO accounts (description,"accountsTypesid") VALUES
	 ('Efectivo',1),
	 ('ING 2351',2),
	 ('ING 0723',2),
	 ('Ikea Family',3),
	 ('Coinbase',4),
	 ('Renta4',4),
	 ('Ptmo ING 0887',5),
	 ('Vivienda ',6),
	 ('Coche',6);


INSERT INTO categories (description) VALUES
	 ('Sueldo'),
	 ('Bares'),
	 ('Comida');

INSERT INTO persons ("name") VALUES
	 ('Nartexsoft'),
	 ('Mercadona'),
	 ('Alcampo');



