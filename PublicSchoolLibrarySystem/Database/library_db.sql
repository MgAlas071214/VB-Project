CREATE DATABASE IF NOT EXISTS library_db;
USE library_db;

DROP TABLE IF EXISTS tbl_activity_logs;
DROP TABLE IF EXISTS tbl_borrowed;
DROP TABLE IF EXISTS tbl_students;
DROP TABLE IF EXISTS tbl_books;
DROP TABLE IF EXISTS tbl_users;

CREATE TABLE tbl_users (
  user_id INT AUTO_INCREMENT PRIMARY KEY,
  fullname VARCHAR(100),
  username VARCHAR(50) UNIQUE,
  password VARCHAR(255),
  role ENUM('Admin','Librarian')
);

CREATE TABLE tbl_books (
  book_id INT AUTO_INCREMENT PRIMARY KEY,
  title VARCHAR(100),
  author VARCHAR(100),
  category VARCHAR(50),
  isbn VARCHAR(20),
  quantity INT DEFAULT 0
);

CREATE TABLE tbl_students (
  student_id INT AUTO_INCREMENT PRIMARY KEY,
  fullname VARCHAR(100),
  grade_level VARCHAR(50),
  section VARCHAR(50),
  contact VARCHAR(50)
);

CREATE TABLE tbl_borrowed (
  borrow_id INT AUTO_INCREMENT PRIMARY KEY,
  student_id INT,
  book_id INT,
  borrow_date DATE,
  return_date DATE,
  status VARCHAR(20),
  FOREIGN KEY (student_id) REFERENCES tbl_students(student_id) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (book_id) REFERENCES tbl_books(book_id)
);

CREATE TABLE tbl_activity_logs (
  log_id INT AUTO_INCREMENT PRIMARY KEY,
  action VARCHAR(255),
  user VARCHAR(100),
  log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO tbl_users (fullname, username, password, role) VALUES
('System Admin', 'admin', 'admin123', 'Admin'),
('School Librarian', 'librarian', 'lib123', 'Librarian');

-- Seed initial books
INSERT INTO tbl_books (title, author, category, isbn, quantity) VALUES
('To Kill a Mockingbird', 'Harper Lee', 'Literature', '9780061120084', 5),
('1984', 'George Orwell', 'Literature', '9780451524935', 4),
('The Great Gatsby', 'F. Scott Fitzgerald', 'Literature', '9780743273565', 3),
('Introduction to Algorithms', 'Thomas H. Cormen', 'Computer Science', '9780262033848', 2),
('Clean Code', 'Robert C. Martin', 'Computer Science', '9780132350884', 3),
('Thinking, Fast and Slow', 'Daniel Kahneman', 'Psychology', '9780374533557', 2),
('A Brief History of Time', 'Stephen Hawking', 'Science', '9780553380163', 3),
('Sapiens: A Brief History of Humankind', 'Yuval Noah Harari', 'History', '9780062316097', 2),
('The Art of War', 'Sun Tzu', 'Philosophy', '9781590302255', 5),
('The Pragmatic Programmer', 'Andrew Hunt; David Thomas', 'Computer Science', '9780201616224', 2),
('Calculus Made Easy', 'Silvanus P. Thompson', 'Mathematics', '9780312185480', 4),
('The Immortal Life of Henrietta Lacks', 'Rebecca Skloot', 'Biography', '9781400052189', 2);
