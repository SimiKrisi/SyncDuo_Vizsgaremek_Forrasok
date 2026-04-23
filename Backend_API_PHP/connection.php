<?php

// --- Adatbázis adatok és kapcsolat létrehozása ---
$host = "localhost";
$dbname = "mesterremekdb4";
$username = "root";
$password = "";

// --- PDO kapcsolat létrehozása ---
try {
    $pdo = new PDO(
        "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
        $username,
        $password,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES => false
        ]
    );
} catch (PDOException $e) {
    // --- Adatbázis kapcsolat hiba kezelése ---
    echo json_encode(["error" => "DB connection failed: " . $e->getMessage()]);
    exit;
}
