from PyQt6.QtWidgets import (
    QApplication, QTableWidget, QTableWidgetItem, QVBoxLayout, QWidget
)
from PyQt6.QtCore import Qt
import sys


class EditableGridApp(QWidget):
    def __init__(self):
        super().__init__()

        # Configuración de la ventana
        self.setWindowTitle("Editable Grid with PyQt")
        self.setGeometry(100, 100, 600, 400)

        # Crear el widget de tabla
        self.table = QTableWidget(self)
        self.table.setRowCount(10)
        self.table.setColumnCount(5)
        self.table.setHorizontalHeaderLabels([f"Col {i + 1}" for i in range(5)])

        # Insertar datos ficticios
        for i in range(10):
            for j in range(5):
                self.table.setItem(i, j, QTableWidgetItem(f"Dato {i + 1}, {j + 1}"))

        # Asociar evento de teclado
        self.table.keyPressEvent = self.key_press_event

        # Configurar layout
        layout = QVBoxLayout(self)
        layout.addWidget(self.table)
        self.setLayout(layout)

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
