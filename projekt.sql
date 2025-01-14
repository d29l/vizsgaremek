-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jan 14, 2025 at 09:06 PM
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
-- Database: `projekt`
--
CREATE DATABASE IF NOT EXISTS `projekt` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE `projekt`;

-- --------------------------------------------------------

--
-- Table structure for table `connections`
--

CREATE TABLE `connections` (
  `ConnectionID` int(11) NOT NULL,
  `RequesterID` int(11) NOT NULL,
  `ReceiverID` int(11) NOT NULL,
  `Status` enum('Pending','Accepted','Declined') DEFAULT 'Pending',
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `employers`
--

CREATE TABLE `employers` (
  `EmployerID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `CompanyName` varchar(255) NOT NULL,
  `CompanyAddress` varchar(255) DEFAULT NULL,
  `Industry` varchar(255) DEFAULT NULL,
  `CompanyWebsite` varchar(255) DEFAULT NULL,
  `CompanyDescription` text DEFAULT NULL,
  `EstablishedYear` year(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `employers`
--

INSERT INTO `employers` (`EmployerID`, `UserID`, `CompanyName`, `CompanyAddress`, `Industry`, `CompanyWebsite`, `CompanyDescription`, `EstablishedYear`) VALUES
(1, 1, 'Voomm', '8061 Weeping Birch Street', 'n/a', 'http://people.com.cn/massa.json?neque=in&libero=felis&convallis=eu', 'Phasellus sit amet erat. Nulla tempus. Vivamus in felis eu sapien cursus vestibulum.', '2006'),
(2, 2, 'Ntags', '8709 Moulton Place', 'Business Services', 'https://lycos.com/sed/sagittis/nam/congue.aspx?nulla=quam&suspendisse=nec&potenti=dui&cras=luctus&in=rutrum&purus=nulla&eu=tellus&magna=in&vulputate=sagittis&luctus=dui&cum=vel&sociis=nisl&natoque=duis&penatibus=ac&et=nibh&magnis=fusce&dis=lacus&parturien', 'Fusce consequat. Nulla nisl. Nunc nisl.', '2007'),
(3, 3, 'Youfeed', '62 Esch Junction', 'n/a', 'https://patch.com/a/suscipit/nulla/elit/ac.js?vel=porta&augue=volutpat&vestibulum=quam&ante=pede&ipsum=lobortis&primis=ligula&in=sit&faucibus=amet&orci=eleifend&luctus=pede&et=libero&ultrices=quis&posuere=orci&cubilia=nullam&curae=molestie&donec=nibh&phar', 'Duis consequat dui nec nisi volutpat eleifend. Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus.', '2012'),
(4, 4, 'Bubblemix', '61305 7th Point', 'Major Pharmaceuticals', 'https://ebay.com/elit/ac.aspx?justo=nisl&eu=duis&massa=bibendum&donec=felis&dapibus=sed&duis=interdum&at=venenatis&velit=turpis&eu=enim&est=blandit&congue=mi&elementum=in&in=porttitor&hac=pede&habitasse=justo&platea=eu&dictumst=massa&morbi=donec&vestibulu', 'Nulla ut erat id mauris vulputate elementum. Nullam varius. Nulla facilisi.', '1990'),
(5, 5, 'Ooba', '4 Hintze Alley', 'Major Chemicals', 'https://liveinternet.ru/dui/maecenas/tristique/est/et/tempus.aspx?enim=felis&blandit=ut&mi=at&in=dolor&porttitor=quis&pede=odio&justo=consequat&eu=varius&massa=integer&donec=ac&dapibus=leo&duis=pellentesque&at=ultrices&velit=mattis&eu=odio&est=donec&congu', 'In congue. Etiam justo. Etiam pretium iaculis justo.', '1978');

-- --------------------------------------------------------

--
-- Table structure for table `messages`
--

CREATE TABLE `messages` (
  `MessageID` int(11) NOT NULL,
  `SenderID` int(11) NOT NULL,
  `ReceiverID` int(11) NOT NULL,
  `Content` text NOT NULL,
  `SentAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `posts`
--

CREATE TABLE `posts` (
  `PostID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `Title` text NOT NULL,
  `Content` text NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `Likes` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `posts`
--

INSERT INTO `posts` (`PostID`, `UserID`, `Title`, `Content`, `CreatedAt`, `Likes`) VALUES
(1, 1, 'Nurse Practicioner', 'In congue. Etiam justo. Etiam pretium iaculis justo.', '2025-01-14 10:00:00', 835),
(2, 2, 'Senior Editor', 'Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi.', '2024-12-25 08:30:15', 744),
(3, 3, 'Senior Sales Associate', 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin risus. Praesent lectus.', '2023-07-04 14:45:30', 575),
(4, 4, 'Food Chemist', 'Sed sagittis. Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus.', '2022-11-01 16:00:00', 570),
(5, 5, 'Media Manager I', 'Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus vestibulum sagittis sapien. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.', '2021-05-20 09:15:45', 631);

-- --------------------------------------------------------

--
-- Table structure for table `profiles`
--

CREATE TABLE `profiles` (
  `ProfileID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `FullName` varchar(255) NOT NULL,
  `Headline` varchar(255) DEFAULT NULL,
  `Bio` text DEFAULT NULL,
  `Location` varchar(255) DEFAULT NULL,
  `ProfilePicture` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Role` enum('Employee','Employer','Admin') DEFAULT 'Employee',
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UserID`, `Email`, `Password`, `Role`, `CreatedAt`, `IsActive`) VALUES
(1, 'iayrs0@g.co', '$2a$04$2gwge/kygBAqZfpdPZVXl.4WqWG9Ucwf3fMJbgX4zJMnK/fuhvzUC', 'Employer', '0000-00-00 00:00:00', 1),
(2, 'cewbach1@printfriendly.com', '$2a$04$564BrRWpVoIgnCV8Nx5d6.OZgirZPlRIVEitikEgtSsVnCgjxdi5a', 'Employer', '0000-00-00 00:00:00', 0),
(3, 'ssparkwill2@myspace.com', '$2a$04$1LnwzHxPWbf7Iv5VeRWG0eWTyYueU7Ee7X0ABEQ3b/XR1bSVheGCC', 'Employee', '0000-00-00 00:00:00', 0),
(4, 'ctapply3@topsy.com', '$2a$04$O9yYqGpbnIRD37EyrYwxTuX4AWQuIpAEo74p/EHovjmeVrUcjNpvm', 'Employer', '0000-00-00 00:00:00', 1),
(5, 'iferbrache4@china.com.cn', '$2a$04$NGkRV3zQoTdOlQdwvru7I.KaM64K9ZID3fl5rpdirLF4ExlFCChmC', 'Employee', '0000-00-00 00:00:00', 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `connections`
--
ALTER TABLE `connections`
  ADD PRIMARY KEY (`ConnectionID`),
  ADD UNIQUE KEY `idx_unique_connection` (`RequesterID`,`ReceiverID`),
  ADD KEY `ReceiverID` (`ReceiverID`);

--
-- Indexes for table `employers`
--
ALTER TABLE `employers`
  ADD PRIMARY KEY (`EmployerID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `messages`
--
ALTER TABLE `messages`
  ADD PRIMARY KEY (`MessageID`),
  ADD KEY `SenderID` (`SenderID`),
  ADD KEY `ReceiverID` (`ReceiverID`);

--
-- Indexes for table `posts`
--
ALTER TABLE `posts`
  ADD PRIMARY KEY (`PostID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `profiles`
--
ALTER TABLE `profiles`
  ADD PRIMARY KEY (`ProfileID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `connections`
--
ALTER TABLE `connections`
  MODIFY `ConnectionID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `employers`
--
ALTER TABLE `employers`
  MODIFY `EmployerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `messages`
--
ALTER TABLE `messages`
  MODIFY `MessageID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `posts`
--
ALTER TABLE `posts`
  MODIFY `PostID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `profiles`
--
ALTER TABLE `profiles`
  MODIFY `ProfileID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `connections`
--
ALTER TABLE `connections`
  ADD CONSTRAINT `connections_ibfk_1` FOREIGN KEY (`RequesterID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  ADD CONSTRAINT `connections_ibfk_2` FOREIGN KEY (`ReceiverID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `employers`
--
ALTER TABLE `employers`
  ADD CONSTRAINT `employers_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`SenderID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`ReceiverID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `posts`
--
ALTER TABLE `posts`
  ADD CONSTRAINT `posts_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `employers` (`UserID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `profiles`
--
ALTER TABLE `profiles`
  ADD CONSTRAINT `profiles_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
