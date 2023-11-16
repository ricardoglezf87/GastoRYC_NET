ALTER TABLE dateCalendar ADD date TEXT;

DROP TABLE dateCalendar;

CREATE TABLE dateCalendar (
    "id" INTEGER PRIMARY KEY,
    "day" INTEGER,
    "month" INTEGER,
    "year" INTEGER,
    "date" TEXT
);