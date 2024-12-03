import sqlite3
import uuid
from datetime import datetime


class Database:
    def __init__(self, db_name="transacciones.db"):
        self.db_name = db_name
        self.connection = sqlite3.connect(self.db_name)
        self.init_db()

    def init_db(self):
        """Inicializa las tablas de la base de datos si no existen."""
        cursor = self.connection.cursor()

        # Tabla de transacciones
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS transacciones (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                fecha TEXT NOT NULL,
                cuenta TEXT NOT NULL,
                descripcion TEXT,
                transferir TEXT,
                debito REAL DEFAULT 0,
                credito REAL DEFAULT 0,
                saldo REAL DEFAULT 0
            )
        """)

        # Tabla de migraciones
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS migraciones (
                guid TEXT PRIMARY KEY,
                fecha TEXT NOT NULL,
                version TEXT NOT NULL
            )
        """)

        # Registrar primera migración si es la primera vez
        cursor.execute("SELECT COUNT(*) FROM migraciones")
        if cursor.fetchone()[0] == 0:
            self.register_migration("1.0.0")

        self.connection.commit()

    def register_migration(self, version):
        """Registra una nueva migración."""
        cursor = self.connection.cursor()
        cursor.execute("""
            INSERT INTO migraciones (guid, fecha, version)
            VALUES (?, ?, ?)
        """, (str(uuid.uuid4()), datetime.now().isoformat(), version))
        self.connection.commit()

    def get_transacciones(self):
        """Obtiene todas las transacciones de la base de datos."""
        cursor = self.connection.cursor()
        cursor.execute("""
            SELECT fecha, cuenta, descripcion, transferir, debito, credito, saldo
            FROM transacciones
        """)
        return cursor.fetchall()

    def save_transacciones(self, transacciones):
        """Guarda las transacciones en la base de datos."""
        cursor = self.connection.cursor()
        cursor.execute("DELETE FROM transacciones")
        cursor.executemany("""
            INSERT INTO transacciones (fecha, cuenta, descripcion, transferir, debito, credito, saldo)
            VALUES (?, ?, ?, ?, ?, ?, ?)
        """, transacciones)
        self.connection.commit()

    def close(self):
        """Cierra la conexión a la base de datos."""
        self.connection.close()
