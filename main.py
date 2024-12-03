import sqlite3
import uuid
from datetime import datetime
from PyQt6.QtWidgets import (
    QApplication, QTableWidget, QTableWidgetItem, QVBoxLayout, QWidget, QPushButton, QHBoxLayout
)
from PyQt6.QtCore import Qt
import sys


class EditableGridApp(QWidget):
    def __init__(self):
        super().__init__()

        # Configuración de la ventana
        self.setWindowTitle("Transacciones - Estilo GNU Cash")
        self.setGeometry(100, 100, 900, 500)

        # Crear el widget de tabla
        self.table = QTableWidget(self)
        self.table.setRowCount(0)  # Comenzamos sin filas
        self.table.setColumnCount(7)  # Cantidad de columnas

        # Definir encabezados de columnas
        column_headers = ["Fecha", "Cuenta", "Descripción", "Transferir", "Débito", "Crédito", "Saldo"]
        self.table.setHorizontalHeaderLabels(column_headers)

        # Ajustar el tamaño de las columnas
        self.table.setColumnWidth(0, 100)  # Fecha
        self.table.setColumnWidth(1, 150)  # Cuenta
        self.table.setColumnWidth(2, 200)  # Descripción
        self.table.setColumnWidth(3, 150)  # Transferir
        self.table.setColumnWidth(4, 100)  # Débito
        self.table.setColumnWidth(5, 100)  # Crédito
        self.table.setColumnWidth(6, 100)  # Saldo

        # Botones
        self.new_line_button = QPushButton("Nueva Línea")
        self.new_line_button.clicked.connect(self.add_new_line)

        # Layouts
        button_layout = QHBoxLayout()
        button_layout.addWidget(self.new_line_button)

        main_layout = QVBoxLayout(self)
        main_layout.addLayout(button_layout)
        main_layout.addWidget(self.table)

        self.setLayout(main_layout)

        # Conexión a la base de datos
        self.db_connection = self.init_db()

        # Cargar datos al iniciar
        self.load_from_db()

        # Evento de cambio en la tabla
        self.table.itemChanged.connect(self.save_to_db)

    def init_db(self):
        """Inicializa la base de datos SQLite y las tablas necesarias."""
        connection = sqlite3.connect("transacciones.db")
        cursor = connection.cursor()

        # Crear tabla de transacciones si no existe
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

        # Crear tabla de migraciones si no existe
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS migraciones (
                guid TEXT PRIMARY KEY,
                fecha TEXT NOT NULL,
                version TEXT NOT NULL
            )
        """)

        # Registrar la primera migración si es la primera vez
        cursor.execute("SELECT COUNT(*) FROM migraciones")
        if cursor.fetchone()[0] == 0:
            cursor.execute("""
                INSERT INTO migraciones (guid, fecha, version)
                VALUES (?, ?, ?)
            """, (str(uuid.uuid4()), datetime.now().isoformat(), "1.0.0"))
            connection.commit()

        return connection

    def load_from_db(self):
        """Carga los datos de la base de datos en la tabla."""
        cursor = self.db_connection.cursor()
        cursor.execute("SELECT fecha, cuenta, descripcion, transferir, debito, credito, saldo FROM transacciones")
        rows = cursor.fetchall()

        self.table.setRowCount(len(rows))
        for row_index, row_data in enumerate(rows):
            for col_index, value in enumerate(row_data):
                self.table.setItem(row_index, col_index, QTableWidgetItem(str(value)))

    def save_to_db(self):
        """Guarda los datos de la tabla en la base de datos."""
        cursor = self.db_connection.cursor()

        # Limpiar la tabla en la base de datos
        cursor.execute("DELETE FROM transacciones")

        # Insertar los datos actuales
        for row_index in range(self.table.rowCount()):
            row_data = []
            for col_index in range(self.table.columnCount()):
                item = self.table.item(row_index, col_index)
                row_data.append(item.text() if item else "")

            cursor.execute("""
                INSERT INTO transacciones (fecha, cuenta, descripcion, transferir, debito, credito, saldo)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            """, row_data)

        self.db_connection.commit()

    def add_new_line(self):
        """Agrega una nueva línea vacía a la tabla."""
        current_row_count = self.table.rowCount()
        self.table.insertRow(current_row_count)
        for col_index in range(self.table.columnCount()):
            self.table.setItem(current_row_count, col_index, QTableWidgetItem(""))


if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = EditableGridApp()
    window.show()
    sys.exit(app.exec())
