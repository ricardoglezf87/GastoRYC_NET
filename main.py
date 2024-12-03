from PyQt6.QtWidgets import QApplication
from ui import EditableGridApp
import sys

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = EditableGridApp()
    window.show()
    sys.exit(app.exec())
