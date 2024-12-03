from PyQt6.QtWidgets import QWidget, QTableWidget, QTableWidgetItem, QVBoxLayout, QPushButton, QHBoxLayout
from PyQt6.QtCore import Qt
from database import Database


class EditableGridApp(QWidget):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Transacciones - Estilo GNU Cash")
        self.setGeometry(100, 100, 900, 500)

        # Base de datos
        self.db = Database()

        # Tabla
        self.table = QTableWidget(self)
        self.table.setRowCount(0)
        self.table.setColumnCount(7)
        column_headers = ["Fecha", "Cuenta", "Descripción", "Transferir", "Débito", "Crédito", "Saldo"]
        self.table.setHorizontalHeaderLabels(column_headers)

        # Botones
        self.new_line_button = QPushButton("Nueva Línea")
        self.new_line_button.clicked.connect(self.add_new_line)

        # Layout
        button_layout = QHBoxLayout()
        button_layout.addWidget(self.new_line_button)

        main_layout = QVBoxLayout(self)
        main_layout.addLayout(button_layout)
        main_layout.addWidget(self.table)

        self.setLayout(main_layout)

        # Cargar datos iniciales
        self.load_from_db()

        # Guardar automáticamente al editar
        self.table.itemChanged.connect(self.save_to_db)

    def load_from_db(self):
        """Carga los datos de la base de datos en la tabla."""
        rows = self.db.get_transacciones()
        self.table.setRowCount(len(rows))
        for row_index, row_data in enumerate(rows):
            for col_index, value in enumerate(row_data):
                self.table.setItem(row_index, col_index, QTableWidgetItem(str(value)))

    def save_to_db(self):
        """Guarda automáticamente los datos en la base de datos."""
        transacciones = []
        for row_index in range(self.table.rowCount()):
            row_data = []
            for col_index in range(self.table.columnCount()):
                item = self.table.item(row_index, col_index)
                row_data.append(item.text() if item else "")
            transacciones.append(row_data)
        self.db.save_transacciones(transacciones)

    def add_new_line(self):
        """Agrega una nueva línea vacía."""
        current_row_count = self.table.rowCount()
        self.table.insertRow(current_row_count)
        for col_index in range(self.table.columnCount()):
            self.table.setItem(current_row_count, col_index, QTableWidgetItem(""))


if __name__ == "__main__":
    # Esto permite probar el módulo de la interfaz de usuario directamente
    from PyQt6.QtWidgets import QApplication
    import sys
    app = QApplication(sys.argv)
    window = EditableGridApp()
    window.show()
    sys.exit(app.exec())
