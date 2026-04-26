-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 23, 2026 at 11:53 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `mesterremekdb4`
--
CREATE DATABASE IF NOT EXISTS `mesterremekdb4` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `mesterremekdb4`;

-- --------------------------------------------------------

--
-- Table structure for table `daily_challenges`
--

CREATE TABLE `daily_challenges` (
  `dailyc_id` int(10) UNSIGNED NOT NULL,
  `data_json` mediumtext DEFAULT NULL,
  `date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `daily_challenges`
--
 
 
INSERT INTO `daily_challenges` (`dailyc_id`, `data_json`, `date`) VALUES
(1, '{\n      \"levelName\": \"Daily_1\",\n      \"width\": 10,\n      \"height\": 10,\n      \"startPosA\": { \"x\": 1, \"y\": 6 },\n      \"startPosB\": { \"x\": 1, \"y\": 5 },\n      \"finishPosA\": { \"x\": 6, \"y\": 4 },\n      \"finishPosB\": { \"x\": 2, \"y\": 2 },\n      \"enemyPositionsA\": [],\n      \"enemyPositionsB\": [],\n      \"mazeLayoutA\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 0, 0, 1, 1, 1, 0, 1, 1,\n        1, 0, 0, 1, 0, 0, 0, 0, 0, 1,\n        1, 0, 1, 1, 0, 1, 1, 0, 1, 1,\n        1, 1, 0, 0, 0, 1, 0, 0, 1, 1,\n        1, 1, 1, 1, 1, 0, 1, 0, 1, 1,\n        1, 0, 0, 0, 1, 1, 1, 0, 1, 1,\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 1,\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ],\n      \"mazeLayoutB\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 1, 1, 0, 0, 0, 0, 0, 1,\n        1, 0, 0, 0, 1, 1, 1, 1, 0, 1,\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 1,\n        1, 0, 1, 1, 1, 1, 1, 0, 1, 1,\n        1, 0, 0, 0, 1, 1, 1, 0, 1, 1,\n        1, 1, 1, 0, 0, 0, 1, 0, 1, 1,\n        1, 0, 0, 1, 1, 0, 0, 0, 1, 1,\n        1, 1, 0, 0, 0, 1, 1, 1, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ]\n    },', NULL),
(2, '{\n      \"levelName\": \"Daily_2\",\n      \"width\": 11,\n      \"height\": 11,\n      \"startPosA\": { \"x\": 1, \"y\": 1 },\n      \"startPosB\": { \"x\": 6, \"y\": 1 },\n      \"finishPosA\": { \"x\": 9, \"y\": 2 },\n      \"finishPosB\": { \"x\": 1, \"y\": 6 },\n      \"enemyPositionsA\": [],\n      \"enemyPositionsB\": [{\"x\": 9, \"y\": 7}],\n      \"mazeLayoutA\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1,\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\n        1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1,\n        1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1,\n        1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1,\n        1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1,\n        1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ],\n      \"mazeLayoutB\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1,\n        1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1,\n        1, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1,\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1,\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ]\n    },', NULL),
(3, '{\r\n      \"levelName\": \"Daily_3\",\r\n      \"width\": 12,\r\n      \"height\": 12,\r\n      \"startPosA\": { \"x\": 10, \"y\": 7 },\r\n      \"startPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 2, \"y\": 6 },\r\n      \"finishPosB\": { \"x\": 10, \"y\": 10 },\r\n      \"enemyPositionsA\": [],\r\n      \"enemyPositionsB\": [],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1,\r\n        1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(4, '{\n      \"levelName\": \"Daily_4\",\n      \"width\": 13,\n      \"height\": 13,\n      \"startPosA\": { \"x\": 1, \"y\": 5 },\n      \"startPosB\": { \"x\": 3, \"y\": 1 },\n      \"finishPosA\": { \"x\": 10, \"y\": 7 },\n      \"finishPosB\": { \"x\": 5, \"y\": 7 },\n      \"enemyPositionsA\": [{\"x\": 10, \"y\": 10}],\n      \"enemyPositionsB\": [],\n      \"mazeLayoutA\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1,\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,\n        1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1,\n        1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1,\n        1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1,\n        1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1,\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1,\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 1,\n        1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ],\n      \"mazeLayoutB\": [\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\n        1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 1,\n        1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1,\n        1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1,\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1,\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1,\n        1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1,\n        1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1,\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\n      ]\n    }', NULL),
(5, '{\r\n      \"levelName\": \"Daily_5\",\r\n      \"width\": 14,\r\n      \"height\": 14,\r\n      \"startPosA\": { \"x\": 10, \"y\": 4 },\r\n      \"startPosB\": { \"x\": 12, \"y\": 8 },\r\n      \"finishPosA\": { \"x\": 6, \"y\": 1 },\r\n      \"finishPosB\": { \"x\": 10, \"y\": 4 },\r\n      \"enemyPositionsA\": [{\"x\": 1, \"y\": 1}],\r\n      \"enemyPositionsB\": [{\"x\": 1, \"y\": 12}],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1,\r\n        1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1,\r\n        1, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(6, '{\r\n      \"levelName\": \"Daily_6\",\r\n      \"width\": 10,\r\n      \"height\": 10,\r\n      \"startPosA\": { \"x\": 3, \"y\": 5 },\r\n      \"startPosB\": { \"x\": 6, \"y\": 4 },\r\n      \"finishPosA\": { \"x\": 5, \"y\": 2 },\r\n      \"finishPosB\": { \"x\": 2, \"y\": 1 },\r\n      \"enemyPositionsA\": [],\r\n      \"enemyPositionsB\": [],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 1, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 1, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(7, '{\r\n      \"levelName\": \"Daily_7\",\r\n      \"width\": 11,\r\n      \"height\": 11,\r\n      \"startPosA\": { \"x\": 2, \"y\": 8 },\r\n      \"startPosB\": { \"x\": 6, \"y\": 6 },\r\n      \"finishPosA\": { \"x\": 9, \"y\": 2 },\r\n      \"finishPosB\": { \"x\": 3, \"y\": 1 },\r\n      \"enemyPositionsA\": [],\r\n      \"enemyPositionsB\": [],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1,\r\n        1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(8, '{\r\n      \"levelName\": \"Daily_8\",\r\n      \"width\": 12,\r\n      \"height\": 12,\r\n      \"startPosA\": { \"x\": 5, \"y\": 7 },\r\n      \"startPosB\": { \"x\": 1, \"y\": 6 },\r\n      \"finishPosA\": { \"x\": 10, \"y\": 9 },\r\n      \"finishPosB\": { \"x\": 8, \"y\": 5 },\r\n      \"enemyPositionsA\": [{\"x\": 2, \"y\": 10}],\r\n      \"enemyPositionsB\": [],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(9, '{\r\n      \"levelName\": \"Daily_9\",\r\n      \"width\": 13,\r\n      \"height\": 13,\r\n      \"startPosA\": { \"x\": 5, \"y\": 3 },\r\n      \"startPosB\": { \"x\": 2, \"y\": 8 },\r\n      \"finishPosA\": { \"x\": 8, \"y\": 9 },\r\n      \"finishPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"enemyPositionsA\": [],\r\n      \"enemyPositionsB\": [],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,\r\n        1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(10, '{\r\n      \"levelName\": \"Daily_10\",\r\n      \"width\": 14,\r\n      \"height\": 14,\r\n      \"startPosA\": { \"x\": 5, \"y\": 5 },\r\n      \"startPosB\": { \"x\": 1, \"y\": 10 },\r\n      \"finishPosA\": { \"x\": 2, \"y\": 1 },\r\n      \"finishPosB\": { \"x\": 10, \"y\": 12 },\r\n      \"enemyPositionsA\": [{\"x\": 3, \"y\": 8}],\r\n      \"enemyPositionsB\": [{\"x\": 11, \"y\": 11 }],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1,\r\n        1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(11, '{\r\n      \"levelName\": \"Daily_11\",\r\n      \"width\": 10,\r\n      \"height\": 10,\r\n      \"startPosA\": { \"x\": 1, \"y\": 8 },\r\n      \"startPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosB\": { \"x\": 6, \"y\": 6  },\r\n      \"enemyPositionsA\": [ { \"x\": 7, \"y\": 7 }],\r\n      \"enemyPositionsB\": [ { \"x\": 1, \"y\": 8 }],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(12, '{\r\n      \"levelName\": \"Daily_12\",\r\n      \"width\": 10,\r\n      \"height\": 10,\r\n      \"startPosA\": { \"x\": 1, \"y\": 1 },\r\n      \"startPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 8, \"y\": 8 },\r\n      \"finishPosB\": { \"x\": 1, \"y\": 8 },\r\n      \"enemyPositionsA\": [  { \"x\": 2, \"y\": 7 } ],\r\n      \"enemyPositionsB\": [   ],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(13, '{\r\n      \"levelName\": \"Daily_13\",\r\n      \"width\": 10,\r\n      \"height\": 10,\r\n      \"startPosA\": { \"x\": 1, \"y\": 1 },\r\n      \"startPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 8, \"y\": 8 },\r\n      \"finishPosB\": { \"x\": 1, \"y\": 8 },\r\n      \"enemyPositionsA\": [ { \"x\": 6, \"y\": 2 }],\r\n      \"enemyPositionsB\": [ { \"x\": 8, \"y\": 8 } ],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(14, '{\r\n      \"levelName\": \"Daily_14\",\r\n      \"width\": 10,\r\n      \"height\": 10,\r\n      \"startPosA\": { \"x\": 1, \"y\": 8 },\r\n      \"startPosB\": { \"x\": 8, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 6, \"y\": 2 },\r\n      \"finishPosB\": { \"x\": 7, \"y\": 8 },\r\n      \"enemyPositionsA\": [{ \"x\": 8, \"y\": 6 }],\r\n      \"enemyPositionsB\": [ ],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 1, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 0, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(15, '{\r\n      \"levelName\": \"Daily_15\",\r\n      \"width\": 11,\r\n      \"height\": 11,\r\n      \"startPosA\": { \"x\": 1, \"y\": 4 },\r\n      \"startPosB\": { \"x\": 6, \"y\": 5 },\r\n      \"finishPosA\": { \"x\": 9, \"y\": 6 },\r\n      \"finishPosB\": { \"x\": 4, \"y\": 5 },\r\n      \"enemyPositionsA\": [ ],\r\n      \"enemyPositionsB\": [{ \"x\": 9, \"y\": 9 }],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(16, '{\r\n      \"levelName\": \"Daily_16\",\r\n      \"width\": 11,\r\n      \"height\": 11,\r\n      \"startPosA\": { \"x\": 1, \"y\": 9 },\r\n      \"startPosB\": { \"x\": 9, \"y\": 1 },\r\n      \"finishPosA\": { \"x\": 9, \"y\": 1 },\r\n      \"finishPosB\": { \"x\": 1, \"y\": 9 },\r\n      \"enemyPositionsA\": [  ],\r\n      \"enemyPositionsB\": [{ \"x\": 7, \"y\": 9 }],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(17, ' {\r\n      \"levelName\": \"Daily_17\",\r\n      \"width\": 12,\r\n      \"height\": 12,\r\n      \"startPosA\": { \"x\": 1, \"y\": 1 },\r\n      \"startPosB\": { \"x\": 10, \"y\": 10 },\r\n      \"finishPosA\": { \"x\": 10, \"y\": 10 },\r\n      \"finishPosB\": { \"x\": 9, \"y\": 2 },\r\n      \"enemyPositionsA\": [ {\"x\":8,\"y\":8} ],\r\n      \"enemyPositionsB\": [ ],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL),
(18, '{\r\n      \"levelName\": \"Daily_18\",\r\n      \"width\": 12,\r\n      \"height\": 12,\r\n      \"startPosA\": { \"x\": 1, \"y\": 6 },\r\n      \"startPosB\": { \"x\": 10, \"y\": 6 },\r\n      \"finishPosA\": { \"x\": 10, \"y\": 6 },\r\n      \"finishPosB\": { \"x\": 1, \"y\": 6 },\r\n      \"enemyPositionsA\": [{ \"x\": 1, \"y\": 1 }, {\"x\": 10, \"y\":3} ],\r\n      \"enemyPositionsB\": [ {\"x\":3,\"y\":1}  ],\r\n      \"mazeLayoutA\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1,\r\n        1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ],\r\n      \"mazeLayoutB\": [\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1,\r\n        1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1,\r\n        1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1,\r\n        1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1,\r\n        1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,\r\n        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1\r\n      ]\r\n    }', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `leaderboard_dailyc`
--

CREATE TABLE `leaderboard_dailyc` (
  `id` int(10) UNSIGNED NOT NULL,
  `user_id` varchar(64) NOT NULL,
  `dailyc_time` float NOT NULL,
  `date` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `leaderboard_dailyc`
--

INSERT INTO `leaderboard_dailyc` (`id`, `user_id`, `dailyc_time`, `date`) VALUES
(61, 'asdf12312', 21, '2000-01-00'),
(66, 'asdf12313', 154, '2000-01-00'),
(71, 'asdf12314', 122, '2000-01-00'),
(77, 'ac9aa3d5-58ec-4196-8e10-74b916b139e5', 8, '2026-04-23'),
(80, '0ba8c7d0-4334-4e26-8841-e63b89969678', 9.73, '2026-04-23'),
(82, '90054ee1-2b64-48d4-9aa3-258c0ef6497e', 38.53, '2026-04-23'),
(83, '2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 12.85, '2026-04-23');

-- --------------------------------------------------------

--
-- Table structure for table `leaderboard_speedrun`
--

CREATE TABLE `leaderboard_speedrun` (
  `id` int(10) UNSIGNED NOT NULL,
  `user_id` varchar(64) NOT NULL,
  `speedrun_amount` int(11) NOT NULL,
  `date` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `leaderboard_speedrun`
--

INSERT INTO `leaderboard_speedrun` (`id`, `user_id`, `speedrun_amount`, `date`) VALUES
(61, 'asdf12312', 6, '2000-01-21'),
(66, 'asdf12313', 7, '2000-01-24'),
(71, 'asdf12314', 10, '2000-01-17'),
(76, 'ac9aa3d5-58ec-4196-8e10-74b916b139e5', 11, '2026-04-23'),
(79, '0ba8c7d0-4334-4e26-8841-e63b89969678', 8, '2026-04-23'),
(81, '90054ee1-2b64-48d4-9aa3-258c0ef6497e', 0, '2026-04-23'),
(82, '2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 13, '2026-04-23');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `user_id` varchar(64) NOT NULL,
  `game_id` varchar(255) DEFAULT NULL,
  `google_id` varchar(255) DEFAULT NULL,
  `display_name` varchar(100) DEFAULT 'New Player',
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `last_login` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`user_id`, `game_id`, `google_id`, `display_name`, `created_at`, `last_login`, `status`) VALUES
('0ba8c7d0-4334-4e26-8841-e63b89969678', NULL, 'SYyOKYRjCXabqQ7UrTWLYf0Fgvs1', 'ExedraGames', '2026-04-23 13:07:19', '2026-04-23 13:08:01', 1),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', NULL, 'ykBkEjzbg6glR7Tjf0sFqMywEYI2', 'TheBestExaminer', '2026-04-23 21:24:48', '2026-04-23 21:25:02', 1),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', NULL, 'BzztRc4hHffyKzf9YRVCc3KNCGD2', 'Simikrisi', '2026-04-23 13:33:24', '2026-04-23 13:33:24', 1),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', NULL, 'Mock_Google_ID_12345', 'UnityEditorMock', '2026-04-23 07:20:23', '2026-04-23 13:45:11', 1),
('asdf12312', NULL, NULL, 'testuser12', '2026-04-16 13:56:32', '2026-04-16 13:56:32', 1),
('asdf12313', NULL, NULL, 'testuser13', '2026-04-16 13:56:32', '2026-04-16 13:56:32', 1),
('asdf12314', NULL, NULL, 'testuser14', '2026-04-16 13:56:32', '2026-04-16 13:56:32', 1);

-- --------------------------------------------------------

--
-- Table structure for table `user_achievements`
--

CREATE TABLE `user_achievements` (
  `user_id` varchar(64) NOT NULL,
  `status` tinyint(4) DEFAULT 1,
  `achievement_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `user_achievements`
--

INSERT INTO `user_achievements` (`user_id`, `status`, `achievement_id`) VALUES
('0ba8c7d0-4334-4e26-8841-e63b89969678', 2, 0),
('0ba8c7d0-4334-4e26-8841-e63b89969678', 1, 65),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 2, 0),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1, 9),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1, 10),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1, 11),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1, 12),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1, 13),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 0),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 9),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 10),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 11),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 12),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 52),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 2, 65),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 0),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 9),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 10),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 11),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 12),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 13),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 52),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 1, 53),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2, 65),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 1, 66),
('asdf12312', 2, 1),
('asdf12313', 2, 1),
('asdf12314', 2, 1);

-- --------------------------------------------------------

--
-- Table structure for table `user_shop_items`
--

CREATE TABLE `user_shop_items` (
  `user_id` varchar(64) NOT NULL,
  `item_number` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `user_shop_items`
--

INSERT INTO `user_shop_items` (`user_id`, `item_number`) VALUES
('0ba8c7d0-4334-4e26-8841-e63b89969678', 0),
('0ba8c7d0-4334-4e26-8841-e63b89969678', 10),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 0),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 1),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 2),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 10),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 11),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 12),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 20),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 21),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 22),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 0),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 10),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 21),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 0),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 1),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 2),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 3),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 10),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 11),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 13),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 20),
('asdf12312', 1),
('asdf12313', 1),
('asdf12314', 1);

-- --------------------------------------------------------

--
-- Table structure for table `user_stats`
--

CREATE TABLE `user_stats` (
  `user_id` varchar(64) NOT NULL,
  `coins` int(11) DEFAULT 0,
  `best_speedrun_amount` int(11) DEFAULT 0,
  `best_dailyc_time` float DEFAULT 0,
  `levels_completed` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `user_stats`
--

INSERT INTO `user_stats` (`user_id`, `coins`, `best_speedrun_amount`, `best_dailyc_time`, `levels_completed`) VALUES
('0ba8c7d0-4334-4e26-8841-e63b89969678', 1010, 8, 10, 1),
('2ea90a7d-8f44-472d-8080-7537ee3fa9f0', 12700, 13, 12.85, 15),
('90054ee1-2b64-48d4-9aa3-258c0ef6497e', 40, 0, 39, 11),
('ac9aa3d5-58ec-4196-8e10-74b916b139e5', 8190, 11, 8, 16),
('asdf12312', 136, 6, 21, 0),
('asdf12313', 854, 7, 154, 0),
('asdf12314', 690, 10, 122, 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `daily_challenges`
--
ALTER TABLE `daily_challenges`
  ADD PRIMARY KEY (`dailyc_id`);

--
-- Indexes for table `leaderboard_dailyc`
--
ALTER TABLE `leaderboard_dailyc`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_user` (`user_id`);

--
-- Indexes for table `leaderboard_speedrun`
--
ALTER TABLE `leaderboard_speedrun`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_user` (`user_id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `idx_google` (`google_id`),
  ADD UNIQUE KEY `idx_game` (`game_id`);

--
-- Indexes for table `user_achievements`
--
ALTER TABLE `user_achievements`
  ADD PRIMARY KEY (`user_id`,`achievement_id`);

--
-- Indexes for table `user_shop_items`
--
ALTER TABLE `user_shop_items`
  ADD PRIMARY KEY (`user_id`,`item_number`);

--
-- Indexes for table `user_stats`
--
ALTER TABLE `user_stats`
  ADD PRIMARY KEY (`user_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `daily_challenges`
--
ALTER TABLE `daily_challenges`
  MODIFY `dailyc_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `leaderboard_dailyc`
--
ALTER TABLE `leaderboard_dailyc`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=84;

--
-- AUTO_INCREMENT for table `leaderboard_speedrun`
--
ALTER TABLE `leaderboard_speedrun`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=83;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `leaderboard_dailyc`
--
ALTER TABLE `leaderboard_dailyc`
  ADD CONSTRAINT `leaderboard_dailyc_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `leaderboard_speedrun`
--
ALTER TABLE `leaderboard_speedrun`
  ADD CONSTRAINT `leaderboard_speedrun_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `user_achievements`
--
ALTER TABLE `user_achievements`
  ADD CONSTRAINT `user_achievements_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `user_shop_items`
--
ALTER TABLE `user_shop_items`
  ADD CONSTRAINT `user_shop_items_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `user_stats`
--
ALTER TABLE `user_stats`
  ADD CONSTRAINT `user_stats_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
