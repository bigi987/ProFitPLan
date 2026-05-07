-- Скрипт создания базы данных и таблиц для FitPlanPro

CREATE DATABASE IF NOT EXISTS fitplanpro;
USE fitplanpro;

-- Таблица пользователей
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) DEFAULT 'User'
);

-- Таблица приемов пищи
CREATE TABLE IF NOT EXISTS meals (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    name VARCHAR(100) NOT NULL,
    calories INT NOT NULL,
    protein DOUBLE DEFAULT 0,
    fat DOUBLE DEFAULT 0,
    carbs DOUBLE DEFAULT 0,
    date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Таблица тренировок
CREATE TABLE IF NOT EXISTS workouts (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    name VARCHAR(100) NOT NULL,
    duration_minutes INT NOT NULL,
    calories_burned INT NOT NULL,
    date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Начальный администратор (пароль: admin)
INSERT IGNORE INTO users (id, username, password_hash, role) 
VALUES (1, 'admin', 'admin', 'Admin');
