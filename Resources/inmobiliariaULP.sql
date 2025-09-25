-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: inmobiliariaulp
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `contratos`
--

DROP TABLE IF EXISTS `contratos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contratos` (
  `id_contrato` int NOT NULL AUTO_INCREMENT,
  `id_inmueble` int NOT NULL,
  `id_inquilino` int NOT NULL,
  `id_usuario` int NOT NULL,
  `id_usuario_finaliza` int DEFAULT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `monto_mensual` decimal(10,0) NOT NULL,
  `fecha_finalizacion_anticipada` date DEFAULT NULL,
  `multa` decimal(10,0) DEFAULT NULL,
  `estado` enum('vigente','finalizado','rescindido') NOT NULL,
  PRIMARY KEY (`id_contrato`),
  KEY `fk_Contrato_Inquilino1_idx` (`id_inquilino`),
  KEY `fk_Contrato_Inmueble1_idx` (`id_inmueble`),
  KEY `fk_contratos_usuarios1_idx` (`id_usuario`),
  KEY `fk_contratos_usuarios_finaliza` (`id_usuario_finaliza`),
  CONSTRAINT `fk_Contrato_Inmueble1` FOREIGN KEY (`id_inmueble`) REFERENCES `inmuebles` (`id_inmueble`),
  CONSTRAINT `fk_Contrato_Inquilino1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilinos` (`id_inquilino`),
  CONSTRAINT `fk_contratos_usuarios1` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id_usuario`),
  CONSTRAINT `fk_contratos_usuarios_finaliza` FOREIGN KEY (`id_usuario_finaliza`) REFERENCES `usuarios` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contratos`
--

LOCK TABLES `contratos` WRITE;
/*!40000 ALTER TABLE `contratos` DISABLE KEYS */;
INSERT INTO `contratos` VALUES (1,1,2,3,NULL,'2024-07-01','2025-06-30',50000,NULL,NULL,'vigente');
/*!40000 ALTER TABLE `contratos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleados`
--

DROP TABLE IF EXISTS `empleados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleados` (
  `id_empleado` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_empleado`),
  KEY `fk_Empleado_Persona1_idx` (`id_persona`),
  CONSTRAINT `fk_Empleado_Persona1` FOREIGN KEY (`id_persona`) REFERENCES `personas` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleados`
--

LOCK TABLES `empleados` WRITE;
/*!40000 ALTER TABLE `empleados` DISABLE KEYS */;
INSERT INTO `empleados` VALUES (1,1,1),(2,2,1),(3,14,1);
/*!40000 ALTER TABLE `empleados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmuebles`
--

DROP TABLE IF EXISTS `inmuebles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmuebles` (
  `id_inmueble` int NOT NULL AUTO_INCREMENT,
  `direccion` varchar(200) NOT NULL,
  `uso` enum('comercial','residencial') NOT NULL,
  `ambientes` int NOT NULL,
  `coordenadas` varchar(150) NOT NULL,
  `precio_base` decimal(10,0) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1',
  `id_propietario` int NOT NULL,
  `id_tipo` int NOT NULL,
  PRIMARY KEY (`id_inmueble`),
  UNIQUE KEY `direccion_UNIQUE` (`direccion`),
  UNIQUE KEY `coordenadas_UNIQUE` (`coordenadas`),
  KEY `fk_Inmueble_Propietario1_idx` (`id_propietario`),
  KEY `fk_Inmueble_Tipo1_idx` (`id_tipo`),
  CONSTRAINT `fk_Inmueble_Propietario1` FOREIGN KEY (`id_propietario`) REFERENCES `propietarios` (`id_propietario`),
  CONSTRAINT `fk_Inmueble_Tipo1` FOREIGN KEY (`id_tipo`) REFERENCES `tipos` (`id_tipo`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmuebles`
--

LOCK TABLES `inmuebles` WRITE;
/*!40000 ALTER TABLE `inmuebles` DISABLE KEYS */;
INSERT INTO `inmuebles` VALUES (1,'Av. Siempre Viva 742','residencial',3,'-34.6037,-58.3816',120000,1,1,1),(7,'Av. Libertador 123','residencial',3,'31.4201,-64.1888',120000,1,1,1),(8,'Calle Falsa 456','comercial',2,'31.4210,-64.1900',95000,1,2,2),(9,'Boulevard Mitre 789','residencial',4,'31.4220,-64.1910',150000,1,3,3),(10,'Ruta 8 Km 12','comercial',1,'31.4230,-64.1920',80000,1,4,4),(11,'Pasaje Central 321','residencial',2,'31.4240,-64.1930',110000,1,5,5);
/*!40000 ALTER TABLE `inmuebles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilinos`
--

DROP TABLE IF EXISTS `inquilinos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilinos` (
  `id_inquilino` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_inquilino`),
  KEY `fk_inquilinos_personas1_idx` (`id_persona`),
  CONSTRAINT `fk_inquilinos_personas1` FOREIGN KEY (`id_persona`) REFERENCES `personas` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilinos`
--

LOCK TABLES `inquilinos` WRITE;
/*!40000 ALTER TABLE `inquilinos` DISABLE KEYS */;
INSERT INTO `inquilinos` VALUES (1,3,0),(2,4,1),(3,5,1),(4,7,1),(5,9,1),(6,12,1),(7,13,1);
/*!40000 ALTER TABLE `inquilinos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pagos`
--

DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `id_pago` int NOT NULL AUTO_INCREMENT,
  `id_contrato` int NOT NULL,
  `id_usuario` int NOT NULL,
  `fecha_pago` date NOT NULL,
  `numero_pago` varchar(45) NOT NULL,
  `importe` decimal(10,0) NOT NULL,
  `concepto` varchar(100) NOT NULL,
  `estadoPago` enum('aprobado','anulado') NOT NULL,
  PRIMARY KEY (`id_pago`),
  KEY `fk_Pago_Contrato_idx` (`id_contrato`),
  KEY `fk_pagos_usuarios1_idx` (`id_usuario`),
  CONSTRAINT `fk_Pago_Contrato` FOREIGN KEY (`id_contrato`) REFERENCES `contratos` (`id_contrato`),
  CONSTRAINT `fk_pagos_usuarios1` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id_usuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pagos`
--

LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `personas`
--

DROP TABLE IF EXISTS `personas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `personas` (
  `id_persona` int NOT NULL AUTO_INCREMENT,
  `dni` varchar(45) NOT NULL,
  `apellido` varchar(45) NOT NULL,
  `nombre` varchar(45) NOT NULL,
  `telefono` varchar(45) NOT NULL,
  `email` varchar(45) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_persona`),
  UNIQUE KEY `dni_UNIQUE` (`dni`),
  UNIQUE KEY `email_UNIQUE` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `personas`
--

LOCK TABLES `personas` WRITE;
/*!40000 ALTER TABLE `personas` DISABLE KEYS */;
INSERT INTO `personas` VALUES (1,'11111111','Administrador','_','2664','admin@correo.com',1),(2,'00000000','usuario','normal','2664','usuario@correo.com',1),(3,'32168723','Camargo','Ramino Daniel','2664','rcamargo@correo.com',1),(4,'31231313','Suarez','Ignacio','2664','isuarez@correo.com',1),(5,'10000001','García','Juan','2664000001','juan.garcia@ulp.com',1),(6,'10000002','Pérez','Ana','2664000002','ana.perez@ulp.com',1),(7,'10000003','López','Carlos','2664000003','carlos.lopez@ulp.com',1),(8,'10000004','Martínez','Lucía','2664000004','lucia.martinez@ulp.com',1),(9,'10000005','Sánchez','Marcos','2664000005','marcos.sanchez@ulp.com',1),(10,'10000006','Romero','Sofía','2664000006','sofia.romero@ulp.com',1),(11,'10000007','Torres','Diego','2664000007','diego.torres@ulp.com',1),(12,'10000008','Ruiz','Valentina','2664000008','valentina.ruiz@ulp.com',1),(13,'10000009','Alvarez','Matías','2664000009','matias.alvarez@ulp.com',1),(14,'10000010','Fernández','Camila','2664000010','camila.fernandez@ulp.com',1);
/*!40000 ALTER TABLE `personas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietarios`
--

DROP TABLE IF EXISTS `propietarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietarios` (
  `id_propietario` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_propietario`),
  KEY `fk_Propietario_Persona1_idx` (`id_persona`),
  CONSTRAINT `fk_Propietario_Persona1` FOREIGN KEY (`id_persona`) REFERENCES `personas` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietarios`
--

LOCK TABLES `propietarios` WRITE;
/*!40000 ALTER TABLE `propietarios` DISABLE KEYS */;
INSERT INTO `propietarios` VALUES (1,3,1),(2,6,1),(3,8,1),(4,10,1),(5,11,1);
/*!40000 ALTER TABLE `propietarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipos`
--

DROP TABLE IF EXISTS `tipos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipos` (
  `id_tipo` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(45) NOT NULL,
  PRIMARY KEY (`id_tipo`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipos`
--

LOCK TABLES `tipos` WRITE;
/*!40000 ALTER TABLE `tipos` DISABLE KEYS */;
INSERT INTO `tipos` VALUES (1,'Departamento'),(2,'Casa'),(3,'PH'),(4,'Dúplex'),(5,'Triplex'),(6,'Cabaña'),(7,'Local comercial'),(8,'Oficina'),(9,'Galpón'),(10,'Terreno'),(11,'Cochera');
/*!40000 ALTER TABLE `tipos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `id_empleado` int NOT NULL,
  `password` varchar(45) NOT NULL,
  `rol` enum('administrador','empleado') NOT NULL,
  `avatar` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  KEY `fk_usuarios_empleados1_idx` (`id_empleado`),
  CONSTRAINT `fk_usuarios_empleados1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,1,'EK5RqcGHXyeuNHrnfNbU0j7ByFqBnvAlC33e8r5whWw=','administrador','persona_1_perfil.jpg'),(2,2,'EK5RqcGHXyeuNHrnfNbU0j7ByFqBnvAlC33e8r5whWw=','empleado','defaultAvatar.png'),(3,3,'EK5RqcGHXyeuNHrnfNbU0j7ByFqBnvAlC33e8r5whWw=','empleado','defaultAvatar.png');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-09-24 21:14:27
