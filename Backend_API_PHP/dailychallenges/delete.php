<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- 1. id kinyerése GET-ből ---
    $id = getIdFromRequest();
    if (!$id) {
        throw new Exception("Missing id");
    }
    // --- 2. SQL összeállítása és végrehajtás ---
    $sql = "DELETE FROM daily_challenges WHERE dailyc_id = :id";
    // --- 3. Végrehajtás és válasz ---
    executeAndRespondForDelete($pdo, $sql, ['id' => $id]);
} catch (Exception $e) {
    // --- 4. Hibakezelés ---
    sendErrorResponse($e);
}