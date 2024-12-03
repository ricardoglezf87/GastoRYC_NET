from PyQt6.QtWidgets import (
    QApplication, QTableWidget, QTableWidgetItem, QVBoxLayout, QWidget
)
from PyQt6.QtCore import Qt
import sys


class EditableGridApp(QWidget):
    def __init__(self):
        super().__init__()

        # Configuración de la ventana
        self.setWindowTitle("Transacciones - Estilo GNU Cash")
        self.setGeometry(100, 100, 900, 400)

        # Crear el widget de tabla
        self.table = QTableWidget(self)
        self.table.setRowCount(10)  # Cantidad inicial de filas
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

        # Insertar datos ficticios
        self.load_sample_data()

        # Asociar evento de teclado
        self.table.keyPressEvent = self.key_press_event

        # Configurar layout
        layout = QVBoxLayout(self)
        layout.addWidget(self.table)
        self.setLayout(layout)

    def load_sample_data(self):
        """Cargar datos ficticios en la tabla."""
        sample_data = [
            ["01/12/2024", "Banco", "Depósito inicial", "", "1000.00", "", "1000.00"],
            ["02/12/2024", "Caja", "Compra de materiales", "Proveedores", "", "200.00", "800.00"],
            ["03/12/2024", "Banco", "Pago de factura", "Servicios", "", "150.00", "650.00"],
            ["04/12/2024", "Banco", "Ingreso por venta", "", "500.00", "", "1150.00"],
        ]

        for row_index, row_data in enumerate(sample_data):
            for col_index, value in enumerate(row_data):
                self.table.setItem(row_index, col_index, QTableWidgetItem(value))

    def key_press_event(self, event):
        """Manejador de eventos de teclado personalizado."""
        if event.key() == Qt.Key.Key_Return:  # Enter
            # Obtener la celda seleccionada
            current_item = self.table.currentItem()
            if current_item:
                self.table.editItem(current_item)  # Activar edición
        else:
            # Si no es Enter, manejar el evento como de costumbre
            super(QTableWidget, self.table).keyPressEvent(event)


if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = EditableGridApp()
    window.show()
    sys.exit(app.exec())
