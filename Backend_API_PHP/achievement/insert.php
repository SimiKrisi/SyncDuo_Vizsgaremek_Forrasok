<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';


try {
    // --- 1. JSON beolvasása és validálása ---
    $parsed = parseAndValidateJsonForInsert($allowedAchievement);
    // --- 2. SQL összeállítása ---
    $sql = "INSERT INTO user_achievements (" . implode(", ", $parsed["columns"]) . ") VALUES (" . implode(", ", $parsed["placeholders"]) . ")";
    // --- 3. Végrehajtás és válasz ---
    executeAndRespondForInsert($pdo, $sql, $parsed["values"]);
} catch (Exception $e) {
    // --- 4. Hibakezelés ---
    sendErrorResponse($e);
}