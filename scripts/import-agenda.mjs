const API = 'http://localhost:5000/api/v1';
const LOGIN_EMAIL = 'pedagogo@sistemadecontrole.local';
const LOGIN_SENHA = 'TrocarSenha123!';

const PERIODOS = {
  '07:00': ['07:00:00', '08:20:00'],
  '08:20': ['08:20:00', '09:20:00'],
  '09:20': ['09:20:00', '10:20:00'],
  '10:20': ['10:20:00', '11:10:00'],
  '13:00': ['13:00:00', '14:00:00'],
  '14:00': ['14:00:00', '15:00:00'],
  '15:00': ['15:00:00', '15:50:00'],
};

const professores = [
  ['Aroldo Barros', 'SOCIO, FILO, CHSA'],
  ['Davi Alexandre', 'GEO, PINTE, HCID'],
  ['Fabio Lima', 'GEO'],
  ['Jouber Silva', 'GEO'],
  ['Lilian F Schuller', 'CHSA, GEO'],
  ['Raydiene Fonseca', 'CHSA, HIST, PINTE'],
  ['Stephenie do Vale', 'HCID, HIST'],
  ['Thaiana Pires', 'HIST'],
  ['Andre Gustavo', 'QUIM'],
  ['Andrews Borges', 'BIOL, CIEN, LCIEN'],
  ['Geisiele', 'LCIEN, FIS'],
  ['Ivan Lima', 'QUIM'],
  ['Jeanne Mendes', 'CIEN, LCIEN, BIOL'],
  ['Paula Mayara', 'ENSREL, LCIEN'],
  ['Priscila Veloso', 'PINTE, CIEN'],
  ['Wiber Gester', 'FIS, LCIEN'],
  ['Alessandra Araujo', 'LP, LETLG, LETLIT'],
  ['Andrea Sanches', 'LETLG, LP, LETLIT'],
  ['Angelo Lima', 'INGL'],
  ['Camila Cunha', 'INGL'],
  ['Cecilia Maquine', 'LP'],
  ['Debora Castro', 'LP, LETLG, LETLIT'],
  ['Edelania Cassia', 'LP, LETLIT'],
  ['Franca Viana', 'ART'],
  ['Hugo Esteves', 'ED FIS, PINLIN'],
  ['Isa Cristina', 'LETLG, LP'],
  ['Jessica Silva', 'PINLIN, ART, ED FIS, ARTMOV'],
  ['Andreza Nogueira', 'MAT, MATTEC'],
  ['Caio', 'MAT, LETMAT'],
  ['Cleide Pereira', 'MAT'],
  ['Eglison Barreto', 'LETMAT, MAT'],
  ['Roberta Souza', 'MAT, LETMAT'],
  ['Ronald Ribeiro', 'MATTEC, MAT, LETMAT'],
  ['Ruberval Lima', 'MAT, LETMAT'],
  ['Telma Moutinho', 'LETMAT, MAT'],
];

// [professor, dia, hora, turma, materia]
const aulas = [
  // Aroldo Barros
  ['Aroldo Barros','Segunda','07:00','1º1','SOCIO'],['Aroldo Barros','Segunda','08:20','2º1','SOCIO'],
  ['Aroldo Barros','Segunda','09:20','2º2','SOCIO'],['Aroldo Barros','Segunda','10:20','3º3','FILO'],
  ['Aroldo Barros','Segunda','14:00','1º2','SOCIO'],
  ['Aroldo Barros','Terca','07:00','2º3','FILO'],['Aroldo Barros','Terca','08:20','3º3','SOCIO'],
  ['Aroldo Barros','Terca','09:20','3º2','FILO'],['Aroldo Barros','Terca','10:20','2º2','FILO'],
  ['Aroldo Barros','Terca','14:00','1º1','FILO'],['Aroldo Barros','Terca','15:00','2º1','FILO'],
  ['Aroldo Barros','Quinta','07:00','3º1','FILO'],['Aroldo Barros','Quinta','08:20','3º2','SOCIO'],
  ['Aroldo Barros','Quinta','09:20','3º1','SOCIO'],['Aroldo Barros','Quinta','13:00','2º3','CHSA'],
  ['Aroldo Barros','Sexta','07:00','1º3','FILO'],['Aroldo Barros','Sexta','08:20','2º3','SOCIO'],
  ['Aroldo Barros','Sexta','09:20','1º4','FILO'],['Aroldo Barros','Sexta','13:00','1º4','SOCIO'],
  ['Aroldo Barros','Sexta','14:00','1º2','FILO'],['Aroldo Barros','Sexta','15:00','2º3','CHSA'],

  // Davi Alexandre
  ['Davi Alexandre','Segunda','07:00','6º2','PINTE'],['Davi Alexandre','Segunda','08:20','6º1','GEO'],
  ['Davi Alexandre','Segunda','09:20','6º2','GEO'],['Davi Alexandre','Segunda','10:20','7º1','GEO'],
  ['Davi Alexandre','Segunda','13:00','7º2','GEO'],['Davi Alexandre','Segunda','15:00','6º3','GEO'],
  ['Davi Alexandre','Terca','07:00','6º1','GEO'],['Davi Alexandre','Terca','08:20','6º1','PINTE'],
  ['Davi Alexandre','Terca','13:00','7º3','GEO'],['Davi Alexandre','Terca','15:00','9º1','HCID'],
  ['Davi Alexandre','Quarta','13:00','7º1','GEO'],['Davi Alexandre','Quarta','14:00','6º3','GEO'],
  ['Davi Alexandre','Quinta','07:00','6º1','PINTE'],['Davi Alexandre','Quinta','08:20','6º1','GEO'],
  ['Davi Alexandre','Quinta','09:20','9º1','HCID'],['Davi Alexandre','Quinta','13:00','7º3','GEO'],
  ['Davi Alexandre','Quinta','14:00','6º3','GEO'],['Davi Alexandre','Quinta','15:00','7º2','GEO'],
  ['Davi Alexandre','Sexta','07:00','6º2','GEO'],['Davi Alexandre','Sexta','08:20','7º3','GEO'],
  ['Davi Alexandre','Sexta','10:20','6º2','PINTE'],['Davi Alexandre','Sexta','13:00','7º2','GEO'],
  ['Davi Alexandre','Sexta','15:00','7º1','GEO'],

  // Fabio Lima
  ['Fabio Lima','Segunda','07:00','9º4','GEO'],['Fabio Lima','Segunda','09:20','9º2','GEO'],
  ['Fabio Lima','Segunda','10:20','9º1','GEO'],
  ['Fabio Lima','Terca','07:00','9º3','GEO'],['Fabio Lima','Terca','08:20','9º1','GEO'],
  ['Fabio Lima','Terca','09:20','9º4','GEO'],['Fabio Lima','Terca','10:20','9º2','GEO'],
  ['Fabio Lima','Quarta','10:20','9º4','GEO'],
  ['Fabio Lima','Quinta','07:00','9º1','GEO'],['Fabio Lima','Quinta','08:20','9º3','GEO'],
  ['Fabio Lima','Quinta','09:20','9º3','GEO'],['Fabio Lima','Quinta','10:20','9º2','GEO'],

  // Jouber Silva
  ['Jouber Silva','Segunda','07:00','8º2','GEO'],['Jouber Silva','Segunda','09:20','8º3','GEO'],
  ['Jouber Silva','Segunda','10:20','8º1','GEO'],
  ['Jouber Silva','Terca','07:00','8º1','GEO'],['Jouber Silva','Terca','09:20','8º4','GEO'],
  ['Jouber Silva','Quarta','08:20','8º3','GEO'],['Jouber Silva','Quarta','10:20','8º4','GEO'],
  ['Jouber Silva','Quinta','07:00','8º1','GEO'],['Jouber Silva','Quinta','09:20','8º2','GEO'],
  ['Jouber Silva','Quinta','10:20','8º4','GEO'],
  ['Jouber Silva','Sexta','07:00','8º3','GEO'],['Jouber Silva','Sexta','08:20','8º4','GEO'],
  ['Jouber Silva','Sexta','10:20','8º2','GEO'],

  // Lilian F Schuller
  ['Lilian F Schuller','Segunda','07:00','1º2','CHSA'],['Lilian F Schuller','Segunda','08:20','3º1','GEO'],
  ['Lilian F Schuller','Segunda','09:20','1º2','GEO'],['Lilian F Schuller','Segunda','10:20','1º3','GEO'],
  ['Lilian F Schuller','Segunda','13:00','1º1','CHSA'],['Lilian F Schuller','Segunda','14:00','2º1','GEO'],
  ['Lilian F Schuller','Segunda','15:00','1º3','CHSA'],
  ['Lilian F Schuller','Terca','07:00','1º4','GEO'],['Lilian F Schuller','Terca','08:20','3º2','GEO'],
  ['Lilian F Schuller','Terca','09:20','1º1','GEO'],['Lilian F Schuller','Terca','10:20','3º3','GEO'],
  ['Lilian F Schuller','Quarta','08:20','1º3','CHSA'],['Lilian F Schuller','Quarta','09:20','1º1','CHSA'],
  ['Lilian F Schuller','Quarta','10:20','1º2','CHSA'],['Lilian F Schuller','Quarta','14:00','1º3','GEO'],
  ['Lilian F Schuller','Quarta','15:00','2º2','GEO'],
  ['Lilian F Schuller','Quinta','07:00','1º2','GEO'],['Lilian F Schuller','Quinta','09:20','2º3','GEO'],
  ['Lilian F Schuller','Quinta','10:20','1º4','GEO'],['Lilian F Schuller','Quinta','14:00','1º3','CHSA'],
  ['Lilian F Schuller','Sexta','07:00','3º2','GEO'],['Lilian F Schuller','Sexta','08:20','3º3','GEO'],
  ['Lilian F Schuller','Sexta','09:20','3º1','GEO'],['Lilian F Schuller','Sexta','10:20','1º1','CHSA'],
  ['Lilian F Schuller','Sexta','13:00','1º2','CHSA'],['Lilian F Schuller','Sexta','14:00','1º3','CHSA'],
  ['Lilian F Schuller','Sexta','15:00','1º1','GEO'],

  // Raydiene Fonseca
  ['Raydiene Fonseca','Segunda','07:00','2º1','CHSA'],['Raydiene Fonseca','Segunda','08:20','6º2','HIST'],
  ['Raydiene Fonseca','Segunda','09:20','6º1','HIST'],['Raydiene Fonseca','Segunda','10:20','6º3','HIST'],
  ['Raydiene Fonseca','Segunda','13:00','2º2','CHSA'],['Raydiene Fonseca','Segunda','14:00','1º4','CHSA'],
  ['Raydiene Fonseca','Segunda','15:00','8º1','HIST'],
  ['Raydiene Fonseca','Terca','07:00','6º3','HIST'],['Raydiene Fonseca','Terca','08:20','6º3','PINTE'],
  ['Raydiene Fonseca','Terca','09:20','6º1','HIST'],['Raydiene Fonseca','Terca','10:20','7º1','HIST'],
  ['Raydiene Fonseca','Terca','13:00','7º2','HIST'],['Raydiene Fonseca','Terca','15:00','7º3','HIST'],
  ['Raydiene Fonseca','Quarta','07:00','1º4','CHSA'],['Raydiene Fonseca','Quarta','08:20','6º2','HIST'],
  ['Raydiene Fonseca','Quarta','09:20','7º3','HIST'],['Raydiene Fonseca','Quarta','14:00','8º1','HIST'],
  ['Raydiene Fonseca','Quarta','15:00','2º2','CHSA'],
  ['Raydiene Fonseca','Quinta','07:00','6º3','PINTE'],['Raydiene Fonseca','Quinta','08:20','7º1','HIST'],
  ['Raydiene Fonseca','Quinta','09:20','7º3','HIST'],['Raydiene Fonseca','Quinta','10:20','7º2','HIST'],
  ['Raydiene Fonseca','Quinta','13:00','6º3','HIST'],['Raydiene Fonseca','Quinta','14:00','6º3','HIST'],
  ['Raydiene Fonseca','Quinta','15:00','6º1','HIST'],
  ['Raydiene Fonseca','Sexta','07:00','2º2','CHSA'],['Raydiene Fonseca','Sexta','08:20','1º4','CHSA'],
  ['Raydiene Fonseca','Sexta','09:20','7º2','HIST'],['Raydiene Fonseca','Sexta','10:20','6º2','HIST'],
  ['Raydiene Fonseca','Sexta','13:00','7º1','HIST'],['Raydiene Fonseca','Sexta','14:00','2º1','CHSA'],
  ['Raydiene Fonseca','Sexta','15:00','8º1','HIST'],

  // Stephenie do Vale
  ['Stephenie do Vale','Segunda','07:00','9º2','HCID'],['Stephenie do Vale','Segunda','08:20','9º2','HIST'],
  ['Stephenie do Vale','Segunda','09:20','9º3','HCID'],['Stephenie do Vale','Segunda','10:20','9º3','HIST'],
  ['Stephenie do Vale','Segunda','13:00','9º4','HIST'],['Stephenie do Vale','Segunda','14:00','2º2','HIST'],
  ['Stephenie do Vale','Segunda','15:00','9º4','HCID'],
  ['Stephenie do Vale','Terca','09:20','2º1','HIST'],['Stephenie do Vale','Terca','10:20','1º1','HIST'],
  ['Stephenie do Vale','Terca','13:00','1º2','HIST'],['Stephenie do Vale','Terca','14:00','1º3','HIST'],
  ['Stephenie do Vale','Terca','15:00','1º4','HIST'],
  ['Stephenie do Vale','Quarta','13:00','9º4','HIST'],['Stephenie do Vale','Quarta','14:00','9º3','HIST'],
  ['Stephenie do Vale','Quarta','15:00','9º2','HIST'],
  ['Stephenie do Vale','Quinta','09:20','9º4','HCID'],['Stephenie do Vale','Quinta','10:20','9º2','HCID'],
  ['Stephenie do Vale','Quinta','13:00','1º4','HIST'],['Stephenie do Vale','Quinta','14:00','2º2','HIST'],
  ['Stephenie do Vale','Quinta','15:00','9º3','HCID'],
  ['Stephenie do Vale','Sexta','07:00','9º2','HIST'],['Stephenie do Vale','Sexta','08:20','9º3','HIST'],
  ['Stephenie do Vale','Sexta','09:20','2º1','HIST'],['Stephenie do Vale','Sexta','10:20','9º4','HIST'],
  ['Stephenie do Vale','Sexta','13:00','1º3','HIST'],['Stephenie do Vale','Sexta','14:00','1º1','HIST'],
  ['Stephenie do Vale','Sexta','15:00','1º2','HIST'],

  // Thaiana Pires
  ['Thaiana Pires','Segunda','07:00','8º3','HIST'],['Thaiana Pires','Segunda','08:20','8º2','HIST'],
  ['Thaiana Pires','Segunda','09:20','9º1','HIST'],['Thaiana Pires','Segunda','10:20','8º4','HIST'],
  ['Thaiana Pires','Terca','10:20','2º3','HIST'],
  ['Thaiana Pires','Quarta','07:00','3º1','HIST'],['Thaiana Pires','Quarta','08:20','8º4','HIST'],
  ['Thaiana Pires','Quarta','09:20','9º1','HIST'],['Thaiana Pires','Quarta','10:20','8º2','HIST'],
  ['Thaiana Pires','Quinta','07:00','2º3','HIST'],['Thaiana Pires','Quinta','08:20','3º3','HIST'],
  ['Thaiana Pires','Quinta','09:20','3º2','HIST'],['Thaiana Pires','Quinta','10:20','8º3','HIST'],
  ['Thaiana Pires','Sexta','07:00','8º2','HIST'],['Thaiana Pires','Sexta','08:20','8º3','HIST'],
  ['Thaiana Pires','Sexta','09:20','8º4','HIST'],['Thaiana Pires','Sexta','10:20','9º1','HIST'],

  // Andre Gustavo
  ['Andre Gustavo','Segunda','07:00','2º2','QUIM'],['Andre Gustavo','Segunda','08:20','3º3','QUIM'],
  ['Andre Gustavo','Segunda','09:20','2º3','QUIM'],['Andre Gustavo','Segunda','10:20','2º1','QUIM'],
  ['Andre Gustavo','Terca','07:00','3º2','QUIM'],['Andre Gustavo','Terca','08:20','2º1','QUIM'],
  ['Andre Gustavo','Terca','09:20','2º3','QUIM'],['Andre Gustavo','Terca','10:20','3º1','QUIM'],
  ['Andre Gustavo','Quarta','07:00','2º2','QUIM'],['Andre Gustavo','Quarta','08:20','3º1','QUIM'],
  ['Andre Gustavo','Quarta','09:20','3º3','QUIM'],['Andre Gustavo','Quarta','10:20','3º2','QUIM'],

  // Andrews Borges
  ['Andrews Borges','Segunda','07:00','3º2','BIOL'],['Andrews Borges','Segunda','08:20','2º3','BIOL'],
  ['Andrews Borges','Segunda','09:20','3º3','BIOL'],['Andrews Borges','Segunda','10:20','6º2','CIEN'],
  ['Andrews Borges','Segunda','13:00','7º3','LCIEN'],['Andrews Borges','Segunda','14:00','6º1','CIEN'],
  ['Andrews Borges','Segunda','15:00','2º3','LCIEN'],
  ['Andrews Borges','Terca','08:20','2º2','BIOL'],['Andrews Borges','Terca','09:20','3º1','BIOL'],
  ['Andrews Borges','Terca','10:20','6º2','CIEN'],['Andrews Borges','Terca','13:00','6º3','CIEN'],
  ['Andrews Borges','Quarta','07:00','6º1','CIEN'],['Andrews Borges','Quarta','08:20','2º2','BIOL'],
  ['Andrews Borges','Quarta','09:20','6º3','CIEN'],['Andrews Borges','Quarta','10:20','6º3','CIEN'],
  ['Andrews Borges','Quarta','13:00','6º1','CIEN'],['Andrews Borges','Quarta','14:00','2º3','LCIEN'],
  ['Andrews Borges','Quarta','15:00','2º3','BIOL'],
  ['Andrews Borges','Quinta','07:00','3º3','BIOL'],['Andrews Borges','Quinta','08:20','3º1','BIOL'],
  ['Andrews Borges','Quinta','10:20','3º2','BIOL'],['Andrews Borges','Quinta','14:00','2º3','LCIEN'],
  ['Andrews Borges','Sexta','10:20','6º2','CIEN'],['Andrews Borges','Sexta','13:00','6º2','CIEN'],

  // Geisiele
  ['Geisiele','Segunda','08:20','1º4','LCIEN'],['Geisiele','Segunda','09:20','1º3','FIS'],
  ['Geisiele','Segunda','10:20','1º4','LCIEN'],['Geisiele','Segunda','13:00','9º2','FIS'],
  ['Geisiele','Segunda','14:00','1º3','FIS'],['Geisiele','Segunda','15:00','2º1','LCIEN'],
  ['Geisiele','Terca','07:00','1º3','LCIEN'],['Geisiele','Terca','08:20','1º2','FIS'],
  ['Geisiele','Terca','09:20','1º3','LCIEN'],['Geisiele','Terca','10:20','9º1','FIS'],
  ['Geisiele','Terca','13:00','9º3','FIS'],['Geisiele','Terca','14:00','1º4','LCIEN'],
  ['Geisiele','Terca','15:00','1º2','FIS'],
  ['Geisiele','Quarta','07:00','2º1','LCIEN'],['Geisiele','Quarta','08:20','9º4','FIS'],
  ['Geisiele','Quarta','09:20','2º1','LCIEN'],['Geisiele','Quarta','10:20','1º3','LCIEN'],
  ['Geisiele','Quinta','07:00','1º4','FIS'],['Geisiele','Quinta','08:20','2º1','FIS'],
  ['Geisiele','Quinta','09:20','1º1','FIS'],['Geisiele','Quinta','10:20','2º1','FIS'],
  ['Geisiele','Quinta','14:00','1º1','FIS'],['Geisiele','Quinta','15:00','1º4','FIS'],

  // Ivan Lima
  ['Ivan Lima','Quarta','07:00','9º3','QUIM'],['Ivan Lima','Quarta','09:20','9º2','QUIM'],
  ['Ivan Lima','Quarta','10:20','1º4','QUIM'],['Ivan Lima','Quarta','13:00','1º1','QUIM'],
  ['Ivan Lima','Quarta','14:00','1º4','QUIM'],['Ivan Lima','Quarta','15:00','1º1','QUIM'],
  ['Ivan Lima','Quinta','07:00','1º3','QUIM'],['Ivan Lima','Quinta','08:20','1º2','QUIM'],
  ['Ivan Lima','Quinta','09:20','1º3','QUIM'],['Ivan Lima','Quinta','10:20','1º2','QUIM'],
  ['Ivan Lima','Quinta','13:00','9º1','QUIM'],['Ivan Lima','Quinta','14:00','9º4','QUIM'],

  // Jeanne Mendes
  ['Jeanne Mendes','Segunda','07:00','7º1','CIEN'],['Jeanne Mendes','Segunda','08:20','7º1','LCIEN'],
  ['Jeanne Mendes','Segunda','09:20','1º4','BIOL'],['Jeanne Mendes','Segunda','10:20','1º1','LCIEN'],
  ['Jeanne Mendes','Terca','07:00','7º2','CIEN'],['Jeanne Mendes','Terca','08:20','1º1','LCIEN'],
  ['Jeanne Mendes','Terca','09:20','7º3','CIEN'],['Jeanne Mendes','Terca','10:20','1º3','BIOL'],
  ['Jeanne Mendes','Terca','13:00','1º1','LCIEN'],['Jeanne Mendes','Terca','14:00','7º3','CIEN'],
  ['Jeanne Mendes','Terca','15:00','1º3','BIOL'],
  ['Jeanne Mendes','Quarta','07:00','7º2','LCIEN'],['Jeanne Mendes','Quarta','08:20','7º2','LCIEN'],
  ['Jeanne Mendes','Quarta','09:20','7º1','CIEN'],['Jeanne Mendes','Quarta','10:20','7º1','LCIEN'],
  ['Jeanne Mendes','Quarta','13:00','7º3','CIEN'],['Jeanne Mendes','Quarta','14:00','1º2','BIOL'],
  ['Jeanne Mendes','Quarta','15:00','7º1','CIEN'],
  ['Jeanne Mendes','Quinta','07:00','7º2','CIEN'],['Jeanne Mendes','Quinta','08:20','7º2','CIEN'],
  ['Jeanne Mendes','Quinta','09:20','2º1','BIOL'],['Jeanne Mendes','Quinta','10:20','1º1','BIOL'],
  ['Jeanne Mendes','Quinta','13:00','1º2','BIOL'],['Jeanne Mendes','Quinta','14:00','2º1','BIOL'],
  ['Jeanne Mendes','Quinta','15:00','1º1','BIOL'],

  // Paula Mayara
  ['Paula Mayara','Segunda','07:00','6º1','ENSREL'],['Paula Mayara','Segunda','08:20','9º4','ENSREL'],
  ['Paula Mayara','Segunda','10:20','1º2','LCIEN'],
  ['Paula Mayara','Terca','07:00','9º2','ENSREL'],['Paula Mayara','Terca','08:20','7º2','ENSREL'],
  ['Paula Mayara','Terca','10:20','6º2','ENSREL'],
  ['Paula Mayara','Quarta','07:00','9º1','ENSREL'],['Paula Mayara','Quarta','08:20','6º3','ENSREL'],
  ['Paula Mayara','Quarta','09:20','1º2','LCIEN'],['Paula Mayara','Quarta','10:20','6º2','ENSREL'],
  ['Paula Mayara','Quinta','07:00','8º4','ENSREL'],['Paula Mayara','Quinta','08:20','8º2','ENSREL'],
  ['Paula Mayara','Quinta','09:20','9º3','ENSREL'],['Paula Mayara','Quinta','10:20','8º1','ENSREL'],
  ['Paula Mayara','Sexta','07:00','1º2','LCIEN'],['Paula Mayara','Sexta','08:20','7º1','ENSREL'],
  ['Paula Mayara','Sexta','09:20','8º3','ENSREL'],['Paula Mayara','Sexta','10:20','7º3','ENSREL'],

  // Priscila Veloso
  ['Priscila Veloso','Segunda','07:00','8º1','PINTE'],['Priscila Veloso','Segunda','08:20','8º1','CIEN'],
  ['Priscila Veloso','Segunda','09:20','8º4','PINTE'],['Priscila Veloso','Segunda','10:20','9º4','CIEN'],
  ['Priscila Veloso','Segunda','13:00','8º3','CIEN'],['Priscila Veloso','Segunda','14:00','8º2','CIEN'],
  ['Priscila Veloso','Segunda','15:00','8º3','PINTE'],
  ['Priscila Veloso','Terca','09:20','8º1','CIEN'],['Priscila Veloso','Terca','10:20','8º2','CIEN'],
  ['Priscila Veloso','Terca','13:00','8º4','CIEN'],['Priscila Veloso','Terca','14:00','8º2','PINTE'],
  ['Priscila Veloso','Terca','15:00','8º3','CIEN'],
  ['Priscila Veloso','Quarta','10:20','9º3','CIEN'],['Priscila Veloso','Quarta','13:00','8º3','PINTE'],
  ['Priscila Veloso','Quarta','14:00','8º4','CIEN'],['Priscila Veloso','Quarta','15:00','8º4','PINTE'],
  ['Priscila Veloso','Quinta','09:20','9º2','CIEN'],['Priscila Veloso','Quinta','10:20','9º1','CIEN'],
  ['Priscila Veloso','Quinta','13:00','8º3','CIEN'],['Priscila Veloso','Quinta','14:00','8º2','PINTE'],
  ['Priscila Veloso','Quinta','15:00','8º1','CIEN'],
  ['Priscila Veloso','Sexta','10:20','8º2','CIEN'],['Priscila Veloso','Sexta','13:00','8º2','CIEN'],
  ['Priscila Veloso','Sexta','14:00','8º1','PINTE'],['Priscila Veloso','Sexta','15:00','8º4','CIEN'],

  // Wiber Gester
  ['Wiber Gester','Segunda','07:00','3º1','FIS'],['Wiber Gester','Segunda','08:20','3º2','FIS'],
  ['Wiber Gester','Segunda','09:20','3º1','FIS'],['Wiber Gester','Segunda','10:20','3º2','FIS'],
  ['Wiber Gester','Terca','07:00','2º2','LCIEN'],['Wiber Gester','Terca','10:20','2º2','LCIEN'],
  ['Wiber Gester','Quinta','07:00','2º2','FIS'],['Wiber Gester','Quinta','08:20','2º3','FIS'],
  ['Wiber Gester','Quinta','09:20','2º2','FIS'],['Wiber Gester','Quinta','10:20','2º3','FIS'],
  ['Wiber Gester','Sexta','07:00','3º3','FIS'],['Wiber Gester','Sexta','08:20','2º2','LCIEN'],
  ['Wiber Gester','Sexta','09:20','3º3','FIS'],['Wiber Gester','Sexta','10:20','2º2','LCIEN'],

  // Alessandra Araujo
  ['Alessandra Araujo','Segunda','07:00','6º3','LP'],['Alessandra Araujo','Segunda','08:20','6º3','LP'],
  ['Alessandra Araujo','Segunda','09:20','7º1','LETLG'],['Alessandra Araujo','Segunda','10:20','7º2','LP'],
  ['Alessandra Araujo','Segunda','13:00','8º2','LETLIT'],['Alessandra Araujo','Segunda','14:00','7º3','LP'],
  ['Alessandra Araujo','Segunda','15:00','7º3','LP'],
  ['Alessandra Araujo','Terca','07:00','7º1','LP'],['Alessandra Araujo','Terca','08:20','7º1','LP'],
  ['Alessandra Araujo','Terca','09:20','6º3','LP'],['Alessandra Araujo','Terca','10:20','6º3','LP'],
  ['Alessandra Araujo','Terca','13:00','8º2','LETLIT'],['Alessandra Araujo','Terca','14:00','7º2','LP'],
  ['Alessandra Araujo','Terca','15:00','7º2','LP'],
  ['Alessandra Araujo','Quarta','07:00','7º1','LP'],['Alessandra Araujo','Quarta','08:20','7º1','LP'],
  ['Alessandra Araujo','Quarta','10:20','7º1','LP'],['Alessandra Araujo','Quarta','13:00','7º2','LETLG'],
  ['Alessandra Araujo','Quarta','14:00','7º3','LP'],['Alessandra Araujo','Quarta','15:00','7º3','LP'],
  ['Alessandra Araujo','Quinta','07:00','7º3','LP'],['Alessandra Araujo','Quinta','08:20','7º3','LP'],
  ['Alessandra Araujo','Quinta','09:20','7º1','LP'],['Alessandra Araujo','Quinta','10:20','7º1','LP'],
  ['Alessandra Araujo','Quinta','13:00','7º2','LP'],['Alessandra Araujo','Quinta','14:00','7º2','LP'],
  ['Alessandra Araujo','Quinta','15:00','7º1','LETLG'],
  ['Alessandra Araujo','Sexta','07:00','6º3','LP'],['Alessandra Araujo','Sexta','08:20','6º3','LP'],
  ['Alessandra Araujo','Sexta','10:20','7º2','LETLG'],['Alessandra Araujo','Sexta','13:00','7º3','LETLG'],
  ['Alessandra Araujo','Sexta','14:00','7º3','LETLG'],['Alessandra Araujo','Sexta','15:00','7º2','LP'],

  // Andrea Sanches
  ['Andrea Sanches','Terca','07:00','6º2','LETLG'],['Andrea Sanches','Terca','08:20','6º2','LP'],
  ['Andrea Sanches','Terca','09:20','6º2','LETLG'],['Andrea Sanches','Terca','10:20','6º1','LP'],
  ['Andrea Sanches','Terca','13:00','6º2','LP'],['Andrea Sanches','Terca','14:00','6º1','LP'],
  ['Andrea Sanches','Terca','15:00','6º2','LP'],
  ['Andrea Sanches','Quarta','07:00','6º3','LETLG'],['Andrea Sanches','Quarta','09:20','9º3','LETLG'],
  ['Andrea Sanches','Quarta','10:20','6º1','LP'],['Andrea Sanches','Quarta','13:00','9º3','LETLG'],
  ['Andrea Sanches','Quarta','14:00','6º2','LP'],['Andrea Sanches','Quarta','15:00','8º3','LETLIT'],
  ['Andrea Sanches','Quinta','07:00','6º2','LP'],['Andrea Sanches','Quinta','08:20','6º2','LP'],
  ['Andrea Sanches','Quinta','09:20','6º1','LP'],['Andrea Sanches','Quinta','10:20','6º3','LETLG'],
  ['Andrea Sanches','Quinta','13:00','8º4','LETLIT'],['Andrea Sanches','Quinta','14:00','8º3','LETLIT'],
  ['Andrea Sanches','Quinta','15:00','8º4','LETLIT'],
  ['Andrea Sanches','Sexta','07:00','6º1','LP'],['Andrea Sanches','Sexta','08:20','6º1','LETLG'],
  ['Andrea Sanches','Sexta','09:20','6º1','LP'],['Andrea Sanches','Sexta','10:20','6º1','LETLG'],
  ['Andrea Sanches','Sexta','13:00','9º4','LETLG'],['Andrea Sanches','Sexta','14:00','9º4','LETLG'],
  ['Andrea Sanches','Sexta','15:00','9º4','LETLG'],

  // Angelo Lima
  ['Angelo Lima','Segunda','07:00','3º3','INGL'],['Angelo Lima','Segunda','08:20','1º2','INGL'],
  ['Angelo Lima','Segunda','09:20','9º4','INGL'],['Angelo Lima','Segunda','13:00','2º3','INGL'],
  ['Angelo Lima','Segunda','14:00','2º2','INGL'],
  ['Angelo Lima','Terca','07:00','3º1','INGL'],['Angelo Lima','Terca','08:20','9º3','INGL'],
  ['Angelo Lima','Terca','09:20','1º4','INGL'],['Angelo Lima','Terca','10:20','2º1','INGL'],
  ['Angelo Lima','Terca','13:00','2º3','INGL'],
  ['Angelo Lima','Quarta','07:00','9º4','INGL'],['Angelo Lima','Quarta','08:20','1º1','INGL'],
  ['Angelo Lima','Quarta','09:20','1º3','INGL'],['Angelo Lima','Quarta','10:20','2º2','INGL'],
  ['Angelo Lima','Quarta','15:00','2º1','INGL'],
  ['Angelo Lima','Quinta','07:00','9º3','INGL'],['Angelo Lima','Quinta','08:20','1º4','INGL'],
  ['Angelo Lima','Quinta','09:20','1º2','INGL'],['Angelo Lima','Quinta','10:20','3º2','INGL'],
  ['Angelo Lima','Sexta','07:00','1º1','INGL'],['Angelo Lima','Sexta','08:20','1º3','INGL'],
  ['Angelo Lima','Sexta','09:20','2º3','INGL'],

  // Camila Cunha (removidas duplicatas conflitantes da fonte original: 08:20 8º4 e 10:20 8º2 na Quinta)
  ['Camila Cunha','Quarta','07:00','8º4','INGL'],['Camila Cunha','Quarta','08:20','8º1','INGL'],
  ['Camila Cunha','Quarta','09:20','8º2','INGL'],['Camila Cunha','Quarta','10:20','8º3','INGL'],
  ['Camila Cunha','Quinta','07:00','9º2','INGL'],['Camila Cunha','Quinta','08:20','9º2','INGL'],
  ['Camila Cunha','Quinta','09:20','8º1','INGL'],['Camila Cunha','Quinta','10:20','9º1','INGL'],
  ['Camila Cunha','Quinta','13:00','8º3','INGL'],
  ['Camila Cunha','Sexta','09:20','9º1','INGL'],['Camila Cunha','Sexta','10:20','8º3','INGL'],

  // Cecilia Maquine
  ['Cecilia Maquine','Terca','08:20','3º1','LP'],['Cecilia Maquine','Terca','09:20','3º3','LP'],
  ['Cecilia Maquine','Terca','10:20','3º2','LP'],
  ['Cecilia Maquine','Quarta','09:20','3º2','LP'],['Cecilia Maquine','Quarta','10:20','3º3','LP'],
  ['Cecilia Maquine','Quinta','08:20','3º1','LP'],['Cecilia Maquine','Quinta','09:20','3º3','LP'],
  ['Cecilia Maquine','Quinta','10:20','3º1','LP'],

  // Debora Castro
  ['Debora Castro','Terca','07:00','8º1','LP'],['Debora Castro','Terca','08:20','9º1','LETLG'],
  ['Debora Castro','Terca','09:20','9º2','LETLG'],['Debora Castro','Terca','10:20','8º1','LETLG'],
  ['Debora Castro','Terca','13:00','8º1','LP'],['Debora Castro','Terca','14:00','8º1','LP'],
  ['Debora Castro','Terca','15:00','8º2','LP'],
  ['Debora Castro','Quarta','07:00','8º2','LETLG'],['Debora Castro','Quarta','08:20','9º1','LETLG'],
  ['Debora Castro','Quarta','09:20','8º1','LP'],['Debora Castro','Quarta','10:20','8º2','LP'],
  ['Debora Castro','Quarta','13:00','8º2','LP'],['Debora Castro','Quarta','14:00','8º3','LETLG'],
  ['Debora Castro','Quarta','15:00','8º2','LETLG'],
  ['Debora Castro','Quinta','07:00','8º1','LETLG'],['Debora Castro','Quinta','08:20','8º2','LP'],
  ['Debora Castro','Quinta','09:20','9º2','LETLG'],['Debora Castro','Quinta','10:20','8º1','LP'],
  ['Debora Castro','Quinta','13:00','8º2','LP'],['Debora Castro','Quinta','14:00','8º4','LETLG'],
  ['Debora Castro','Quinta','15:00','8º2','LP'],
  ['Debora Castro','Sexta','08:20','8º2','LP'],['Debora Castro','Sexta','09:20','8º1','LP'],
  ['Debora Castro','Sexta','10:20','8º1','LP'],['Debora Castro','Sexta','13:00','8º1','LP'],
  ['Debora Castro','Sexta','14:00','8º3','LETLG'],

  // Edelania Cassia
  ['Edelania Cassia','Terca','07:00','9º1','LP'],['Edelania Cassia','Terca','08:20','9º2','LP'],
  ['Edelania Cassia','Terca','09:20','9º1','LP'],['Edelania Cassia','Terca','10:20','9º3','LP'],
  ['Edelania Cassia','Terca','13:00','9º1','LP'],['Edelania Cassia','Terca','14:00','8º1','LETLIT'],
  ['Edelania Cassia','Terca','15:00','9º1','LP'],
  ['Edelania Cassia','Quarta','07:00','9º2','LP'],['Edelania Cassia','Quarta','08:20','9º3','LP'],
  ['Edelania Cassia','Quarta','09:20','9º4','LP'],['Edelania Cassia','Quarta','10:20','9º2','LP'],
  ['Edelania Cassia','Quarta','13:00','9º2','LP'],['Edelania Cassia','Quarta','14:00','9º4','LP'],
  ['Edelania Cassia','Quarta','15:00','9º4','LP'],
  ['Edelania Cassia','Quinta','07:00','9º2','LP'],['Edelania Cassia','Quinta','08:20','9º4','LP'],
  ['Edelania Cassia','Quinta','10:20','9º3','LP'],['Edelania Cassia','Quinta','13:00','9º4','LP'],
  ['Edelania Cassia','Quinta','14:00','9º3','LP'],
  ['Edelania Cassia','Sexta','07:00','9º3','LP'],['Edelania Cassia','Sexta','08:20','9º2','LP'],
  ['Edelania Cassia','Sexta','09:20','9º3','LP'],['Edelania Cassia','Sexta','10:20','9º3','LP'],
  ['Edelania Cassia','Sexta','13:00','9º1','LP'],['Edelania Cassia','Sexta','14:00','9º2','LP'],
  ['Edelania Cassia','Sexta','15:00','9º1','LP'],

  // Franca Viana
  ['Franca Viana','Quarta','07:00','1º2','ART'],['Franca Viana','Quarta','08:20','3º2','ART'],
  ['Franca Viana','Quarta','09:20','2º2','ART'],['Franca Viana','Quarta','10:20','2º3','ART'],
  ['Franca Viana','Quinta','09:20','1º4','ART'],['Franca Viana','Quinta','10:20','3º3','ART'],
  ['Franca Viana','Sexta','07:00','2º1','ART'],['Franca Viana','Sexta','08:20','1º1','ART'],
  ['Franca Viana','Sexta','09:20','1º3','ART'],['Franca Viana','Sexta','10:20','3º1','ART'],

  // Hugo Esteves
  ['Hugo Esteves','Terca','13:00','9º1','ED FIS'],['Hugo Esteves','Terca','14:00','9º1','ED FIS'],
  ['Hugo Esteves','Terca','15:00','9º2','ED FIS'],
  ['Hugo Esteves','Quarta','13:00','9º1','PINLIN'],['Hugo Esteves','Quarta','14:00','9º1','PINLIN'],
  ['Hugo Esteves','Quinta','14:00','9º2','PINLIN'],['Hugo Esteves','Quinta','15:00','9º2','PINLIN'],
  ['Hugo Esteves','Sexta','14:00','9º2','ED FIS'],

  // Isa Cristina
  ['Isa Cristina','Segunda','07:00','1º4','LETLG'],['Isa Cristina','Segunda','08:20','1º3','LETLG'],
  ['Isa Cristina','Segunda','13:00','1º2','LETLG'],['Isa Cristina','Segunda','14:00','1º1','LP'],
  ['Isa Cristina','Segunda','15:00','1º1','LP'],
  ['Isa Cristina','Terca','07:00','1º2','LP'],['Isa Cristina','Terca','08:20','1º4','LETLG'],
  ['Isa Cristina','Terca','09:20','1º2','LP'],['Isa Cristina','Terca','10:20','1º2','LETLG'],
  ['Isa Cristina','Terca','13:00','1º3','LETLG'],['Isa Cristina','Terca','14:00','1º2','LETLG'],
  ['Isa Cristina','Terca','15:00','1º1','LETLG'],
  ['Isa Cristina','Quarta','07:00','1º3','LETLG'],['Isa Cristina','Quarta','08:20','1º4','LETLG'],
  ['Isa Cristina','Quarta','10:20','1º3','LETLG'],['Isa Cristina','Quarta','13:00','1º2','LETLG'],
  ['Isa Cristina','Quarta','14:00','1º1','LETLG'],['Isa Cristina','Quarta','15:00','1º4','LETLG'],
  ['Isa Cristina','Quinta','07:00','1º1','LETLG'],['Isa Cristina','Quinta','08:20','1º3','LETLG'],
  ['Isa Cristina','Quinta','10:20','1º3','LETLG'],['Isa Cristina','Quinta','13:00','1º3','LP'],
  ['Isa Cristina','Quinta','14:00','1º2','LETLG'],['Isa Cristina','Quinta','15:00','1º3','LP'],
  ['Isa Cristina','Sexta','07:00','1º1','LETLG'],['Isa Cristina','Sexta','08:20','1º1','LETLG'],
  ['Isa Cristina','Sexta','09:20','1º1','LETLG'],['Isa Cristina','Sexta','10:20','1º4','LP'],
  ['Isa Cristina','Sexta','13:00','1º1','LETLG'],['Isa Cristina','Sexta','14:00','1º4','LP'],
  ['Isa Cristina','Sexta','15:00','1º4','LETLG'],

  // Jessica Silva
  ['Jessica Silva','Segunda','07:00','9º3','PINLIN'],['Jessica Silva','Segunda','08:20','9º3','PINLIN'],
  ['Jessica Silva','Segunda','13:00','9º3','ED FIS'],['Jessica Silva','Segunda','14:00','9º3','ED FIS'],
  ['Jessica Silva','Segunda','15:00','9º3','ED FIS'],
  ['Jessica Silva','Terca','07:00','9º4','PINLIN'],['Jessica Silva','Terca','08:20','9º4','PINLIN'],
  ['Jessica Silva','Terca','09:20','9º3','ART'],['Jessica Silva','Terca','10:20','9º4','ARTMOV'],
  ['Jessica Silva','Terca','13:00','8º3','ED FIS'],['Jessica Silva','Terca','14:00','8º4','ED FIS'],
  ['Jessica Silva','Terca','15:00','8º4','ED FIS'],
  ['Jessica Silva','Quinta','07:00','9º4','ART'],['Jessica Silva','Quinta','08:20','9º2','ART'],
  ['Jessica Silva','Quinta','13:00','8º1','ED FIS'],['Jessica Silva','Quinta','14:00','8º1','ED FIS'],
  ['Jessica Silva','Quinta','15:00','8º3','ED FIS'],
  ['Jessica Silva','Sexta','07:00','9º4','ED FIS'],['Jessica Silva','Sexta','08:20','9º4','ED FIS'],
  ['Jessica Silva','Sexta','09:20','7º1','PINLIN'],['Jessica Silva','Sexta','10:20','7º1','PINLIN'],
  ['Jessica Silva','Sexta','13:00','9º3','ARTMOV'],['Jessica Silva','Sexta','14:00','8º2','ED FIS'],
  ['Jessica Silva','Sexta','15:00','8º2','ED FIS'],

  // Andreza Nogueira
  ['Andreza Nogueira','Segunda','07:00','1º3','MAT'],['Andreza Nogueira','Segunda','08:20','1º1','MATTEC'],
  ['Andreza Nogueira','Segunda','09:20','1º1','MAT'],['Andreza Nogueira','Segunda','10:20','3º1','MAT'],
  ['Andreza Nogueira','Segunda','13:00','1º3','MATTEC'],['Andreza Nogueira','Segunda','15:00','1º2','MATTEC'],
  ['Andreza Nogueira','Terca','07:00','1º1','MATTEC'],['Andreza Nogueira','Terca','08:20','1º3','MATTEC'],
  ['Andreza Nogueira','Terca','10:20','1º1','MAT'],
  ['Andreza Nogueira','Quarta','07:00','1º1','MATTEC'],['Andreza Nogueira','Quarta','08:20','1º2','MAT'],
  ['Andreza Nogueira','Quarta','09:20','3º1','MAT'],['Andreza Nogueira','Quarta','10:20','1º1','MAT'],
  ['Andreza Nogueira','Quarta','13:00','1º3','MAT'],['Andreza Nogueira','Quarta','14:00','1º3','MATTEC'],
  ['Andreza Nogueira','Quarta','15:00','1º2','MATTEC'],
  ['Andreza Nogueira','Quinta','07:00','3º1','MAT'],['Andreza Nogueira','Quinta','08:20','1º2','MAT'],
  ['Andreza Nogueira','Quinta','09:20','1º2','MATTEC'],['Andreza Nogueira','Quinta','10:20','1º3','MATTEC'],
  ['Andreza Nogueira','Quinta','13:00','1º1','MATTEC'],['Andreza Nogueira','Quinta','15:00','1º2','MATTEC'],
  ['Andreza Nogueira','Sexta','07:00','3º1','MAT'],

  // Caio
  ['Caio','Segunda','13:00','7º1','MAT'],['Caio','Segunda','14:00','7º1','LETMAT'],['Caio','Segunda','15:00','7º1','MAT'],
  ['Caio','Terca','13:00','7º1','MAT'],['Caio','Terca','15:00','6º3','MAT'],
  ['Caio','Quarta','13:00','6º3','MAT'],['Caio','Quarta','14:00','7º1','MAT'],['Caio','Quarta','15:00','6º3','MAT'],
  ['Caio','Quinta','13:00','6º3','MAT'],['Caio','Quinta','14:00','7º1','MAT'],['Caio','Quinta','15:00','6º3','MAT'],
  ['Caio','Sexta','13:00','6º3','MAT'],['Caio','Sexta','14:00','7º1','MAT'],['Caio','Sexta','15:00','6º3','MAT'],

  // Cleide Pereira
  ['Cleide Pereira','Segunda','13:00','6º2','MAT'],['Cleide Pereira','Segunda','14:00','6º2','MAT'],['Cleide Pereira','Segunda','15:00','6º1','MAT'],
  ['Cleide Pereira','Terca','13:00','6º2','MAT'],['Cleide Pereira','Terca','14:00','6º2','MAT'],['Cleide Pereira','Terca','15:00','6º1','MAT'],
  ['Cleide Pereira','Quarta','14:00','6º1','MAT'],['Cleide Pereira','Quarta','15:00','6º1','MAT'],
  ['Cleide Pereira','Quinta','13:00','6º1','MAT'],['Cleide Pereira','Quinta','14:00','6º1','MAT'],['Cleide Pereira','Quinta','15:00','6º2','MAT'],
  ['Cleide Pereira','Sexta','13:00','6º1','MAT'],['Cleide Pereira','Sexta','14:00','6º2','MAT'],['Cleide Pereira','Sexta','15:00','6º2','MAT'],

  // Eglison Barreto
  ['Eglison Barreto','Segunda','07:00','7º2','LETMAT'],['Eglison Barreto','Segunda','08:20','9º1','MAT'],
  ['Eglison Barreto','Segunda','09:20','3º2','MAT'],['Eglison Barreto','Segunda','10:20','9º2','MAT'],
  ['Eglison Barreto','Segunda','13:00','6º1','LETMAT'],['Eglison Barreto','Segunda','14:00','9º1','LETMAT'],
  ['Eglison Barreto','Segunda','15:00','6º2','LETMAT'],
  ['Eglison Barreto','Terca','07:00','3º3','MAT'],['Eglison Barreto','Terca','08:20','3º3','MAT'],
  ['Eglison Barreto','Terca','09:20','6º2','LETMAT'],['Eglison Barreto','Terca','10:20','9º4','LETMAT'],
  ['Eglison Barreto','Terca','13:00','6º1','LETMAT'],['Eglison Barreto','Terca','14:00','9º2','LETMAT'],
  ['Eglison Barreto','Terca','15:00','9º2','MAT'],
  ['Eglison Barreto','Quarta','07:00','3º2','MAT'],['Eglison Barreto','Quarta','08:20','3º3','MAT'],
  ['Eglison Barreto','Quarta','09:20','6º2','LETMAT'],['Eglison Barreto','Quarta','10:20','9º4','LETMAT'],
  ['Eglison Barreto','Quarta','13:00','9º2','LETMAT'],['Eglison Barreto','Quarta','15:00','9º1','MAT'],
  ['Eglison Barreto','Quinta','07:00','9º1','MAT'],['Eglison Barreto','Quinta','09:20','9º1','MAT'],
  ['Eglison Barreto','Quinta','10:20','3º3','MAT'],['Eglison Barreto','Quinta','13:00','9º2','MAT'],
  ['Eglison Barreto','Quinta','14:00','9º1','MAT'],['Eglison Barreto','Quinta','15:00','9º1','LETMAT'],
  ['Eglison Barreto','Sexta','07:00','9º1','MAT'],['Eglison Barreto','Sexta','08:20','3º2','MAT'],
  ['Eglison Barreto','Sexta','09:20','9º4','LETMAT'],['Eglison Barreto','Sexta','10:20','3º3','MAT'],
  ['Eglison Barreto','Sexta','13:00','9º2','MAT'],['Eglison Barreto','Sexta','14:00','9º1','LETMAT'],
  ['Eglison Barreto','Sexta','15:00','9º2','LETMAT'],

  // Roberta Souza
  ['Roberta Souza','Segunda','13:00','9º3','MAT'],['Roberta Souza','Segunda','14:00','9º4','MAT'],['Roberta Souza','Segunda','15:00','9º3','LETMAT'],
  ['Roberta Souza','Terca','13:00','9º4','MAT'],['Roberta Souza','Terca','14:00','9º3','MAT'],['Roberta Souza','Terca','15:00','9º3','LETMAT'],
  ['Roberta Souza','Quarta','13:00','9º3','LETMAT'],['Roberta Souza','Quarta','14:00','9º4','MAT'],['Roberta Souza','Quarta','15:00','9º3','MAT'],
  ['Roberta Souza','Quinta','15:00','9º4','MAT'],
  ['Roberta Souza','Sexta','13:00','9º4','LETMAT'],['Roberta Souza','Sexta','14:00','9º3','LETMAT'],['Roberta Souza','Sexta','15:00','9º3','MAT'],

  // Ronald Ribeiro
  ['Ronald Ribeiro','Segunda','07:00','2º1','MATTEC'],['Ronald Ribeiro','Segunda','08:20','2º3','MATTEC'],
  ['Ronald Ribeiro','Segunda','09:20','6º3','LETMAT'],['Ronald Ribeiro','Segunda','10:20','2º2','MAT'],
  ['Ronald Ribeiro','Segunda','13:00','2º1','MATTEC'],['Ronald Ribeiro','Segunda','14:00','2º3','MATTEC'],
  ['Ronald Ribeiro','Segunda','15:00','2º2','MATTEC'],
  ['Ronald Ribeiro','Terca','07:00','2º1','MATTEC'],['Ronald Ribeiro','Terca','08:20','2º3','MATTEC'],
  ['Ronald Ribeiro','Terca','09:20','2º2','MATTEC'],['Ronald Ribeiro','Terca','10:20','1º4','MATTEC'],
  ['Ronald Ribeiro','Terca','13:00','1º4','MAT'],['Ronald Ribeiro','Terca','14:00','2º3','MAT'],
  ['Ronald Ribeiro','Terca','15:00','2º2','MATTEC'],
  ['Ronald Ribeiro','Quarta','07:00','2º3','MATTEC'],['Ronald Ribeiro','Quarta','08:20','2º1','MATTEC'],
  ['Ronald Ribeiro','Quarta','09:20','1º4','MAT'],['Ronald Ribeiro','Quarta','10:20','2º1','MAT'],
  ['Ronald Ribeiro','Quarta','13:00','1º4','MATTEC'],['Ronald Ribeiro','Quarta','14:00','2º2','MATTEC'],
  ['Ronald Ribeiro','Quinta','07:00','1º4','MATTEC'],['Ronald Ribeiro','Quinta','08:20','2º1','MAT'],
  ['Ronald Ribeiro','Quinta','09:20','6º3','LETMAT'],['Ronald Ribeiro','Quinta','13:00','2º3','MAT'],
  ['Ronald Ribeiro','Quinta','14:00','1º4','MATTEC'],['Ronald Ribeiro','Quinta','15:00','2º3','MATTEC'],
  ['Ronald Ribeiro','Sexta','07:00','2º1','MATTEC'],['Ronald Ribeiro','Sexta','08:20','2º1','MATTEC'],
  ['Ronald Ribeiro','Sexta','09:20','6º3','LETMAT'],['Ronald Ribeiro','Sexta','10:20','2º1','MATTEC'],
  ['Ronald Ribeiro','Sexta','13:00','2º2','MAT'],['Ronald Ribeiro','Sexta','14:00','2º2','MAT'],
  ['Ronald Ribeiro','Sexta','15:00','2º2','MATTEC'],

  // Ruberval Lima
  ['Ruberval Lima','Segunda','08:20','7º2','MAT'],['Ruberval Lima','Segunda','09:20','7º2','MAT'],['Ruberval Lima','Segunda','10:20','7º3','MAT'],
  ['Ruberval Lima','Terca','07:00','7º3','MAT'],['Ruberval Lima','Terca','08:20','7º3','MAT'],
  ['Ruberval Lima','Terca','09:20','7º2','MAT'],['Ruberval Lima','Terca','10:20','7º3','LETMAT'],
  ['Ruberval Lima','Quarta','07:00','7º3','MAT'],['Ruberval Lima','Quarta','08:20','7º3','MAT'],
  ['Ruberval Lima','Quarta','09:20','7º2','MAT'],['Ruberval Lima','Quarta','10:20','7º2','MAT'],
  ['Ruberval Lima','Quinta','07:00','7º2','MAT'],['Ruberval Lima','Quinta','09:20','7º3','MAT'],['Ruberval Lima','Quinta','10:20','7º3','MAT'],
  ['Ruberval Lima','Sexta','07:00','7º2','MAT'],['Ruberval Lima','Sexta','08:20','7º3','MAT'],
  ['Ruberval Lima','Sexta','09:20','7º3','MAT'],['Ruberval Lima','Sexta','10:20','7º3','MAT'],

  // Telma Moutinho
  ['Telma Moutinho','Segunda','07:00','8º4','LETMAT'],['Telma Moutinho','Segunda','08:20','8º3','MAT'],
  ['Telma Moutinho','Segunda','09:20','8º1','MAT'],['Telma Moutinho','Segunda','10:20','8º2','MAT'],
  ['Telma Moutinho','Segunda','13:00','8º4','MAT'],['Telma Moutinho','Segunda','14:00','8º3','MAT'],
  ['Telma Moutinho','Segunda','15:00','8º4','LETMAT'],
  ['Telma Moutinho','Terca','07:00','8º2','MAT'],['Telma Moutinho','Terca','08:20','8º2','MAT'],
  ['Telma Moutinho','Terca','09:20','8º3','MAT'],['Telma Moutinho','Terca','10:20','8º2','MAT'],
  ['Telma Moutinho','Terca','13:00','8º1','LETMAT'],['Telma Moutinho','Terca','14:00','8º3','MAT'],
  ['Telma Moutinho','Terca','15:00','8º1','LETMAT'],
  ['Telma Moutinho','Quarta','07:00','8º2','LETMAT'],['Telma Moutinho','Quarta','08:20','8º2','MAT'],
  ['Telma Moutinho','Quarta','09:20','8º3','MAT'],['Telma Moutinho','Quarta','10:20','8º1','MAT'],
  ['Telma Moutinho','Quarta','13:00','8º4','MAT'],['Telma Moutinho','Quarta','14:00','8º2','LETMAT'],
  ['Telma Moutinho','Quarta','15:00','8º1','MAT'],
  ['Telma Moutinho','Quinta','07:00','8º1','MAT'],['Telma Moutinho','Quinta','08:20','8º1','MAT'],
  ['Telma Moutinho','Quinta','09:20','8º3','MAT'],['Telma Moutinho','Quinta','13:00','8º4','MAT'],
  ['Telma Moutinho','Quinta','14:00','8º4','MAT'],['Telma Moutinho','Quinta','15:00','8º3','LETMAT'],
  ['Telma Moutinho','Sexta','07:00','8º1','MAT'],['Telma Moutinho','Sexta','08:20','8º1','MAT'],
  ['Telma Moutinho','Sexta','09:20','8º2','MAT'],['Telma Moutinho','Sexta','10:20','8º4','MAT'],
  ['Telma Moutinho','Sexta','13:00','8º3','LETMAT'],['Telma Moutinho','Sexta','14:00','8º4','MAT'],
  ['Telma Moutinho','Sexta','15:00','8º3','LETMAT'],
];

function slug(nome) {
  return nome.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase().replace(/[^a-z]+/g, '.');
}

async function api(token, method, path, body) {
  const res = await fetch(`${API}${path}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });
  const text = await res.text();
  const data = text ? JSON.parse(text) : null;
  if (!res.ok) {
    const err = new Error(data?.detail || data?.title || res.statusText);
    err.status = res.status;
    err.data = data;
    throw err;
  }
  return data;
}

async function main() {
  console.log('Login...');
  const login = await api(null, 'POST', '/auth/login', { email: LOGIN_EMAIL, senha: LOGIN_SENHA });
  const token = login.token;
  console.log('OK.');

  // ----- Professores -----
  console.log(`\nCriando ${professores.length} professores...`);
  const professorId = new Map();
  let idx = 0;
  for (const [nome, disciplina] of professores) {
    idx++;
    const matricula = `P${String(idx).padStart(3, '0')}`;
    const email = `${slug(nome)}@escola.local`;
    try {
      const criado = await api(token, 'POST', '/professores', { nome, email, matricula, disciplina });
      professorId.set(nome, criado.id);
    } catch (e) {
      if (e.status === 409) {
        const lista = await api(token, 'GET', `/professores?nome=${encodeURIComponent(nome)}`);
        const existente = lista.find((p) => p.nome === nome);
        if (existente) professorId.set(nome, existente.id);
        else console.error(`  Falha ao resolver professor existente: ${nome}`);
      } else {
        console.error(`  Erro ao criar professor ${nome}:`, e.message);
      }
    }
  }
  console.log(`  ${professorId.size} professores prontos.`);

  // ----- Turmas + Salas (uma sala por turma, já que a fonte não define salas) -----
  const codigosTurma = [...new Set(aulas.map((a) => a[3]))].sort();
  console.log(`\nCriando ${codigosTurma.length} turmas e salas...`);
  const turmaId = new Map();
  const salaId = new Map();
  const anoLetivo = new Date().getFullYear();

  for (const codigo of codigosTurma) {
    try {
      const turma = await api(token, 'POST', '/turmas', { nome: codigo, turno: 'Manha', anoLetivo });
      turmaId.set(codigo, turma.id);
    } catch (e) {
      if (e.status === 409) {
        const lista = await api(token, 'GET', `/turmas?anoLetivo=${anoLetivo}`);
        const existente = lista.find((t) => t.nome === codigo);
        if (existente) turmaId.set(codigo, existente.id);
      } else {
        console.error(`  Erro ao criar turma ${codigo}:`, e.message);
      }
    }

    const nomeSala = `Sala ${codigo}`;
    try {
      const sala = await api(token, 'POST', '/salas', { nome: nomeSala });
      salaId.set(codigo, sala.id);
    } catch (e) {
      if (e.status === 409) {
        const lista = await api(token, 'GET', '/salas');
        const existente = lista.find((s) => s.nome === nomeSala);
        if (existente) salaId.set(codigo, existente.id);
      } else {
        console.error(`  Erro ao criar sala ${nomeSala}:`, e.message);
      }
    }
  }
  console.log(`  ${turmaId.size} turmas e ${salaId.size} salas prontas.`);

  // ----- Cronograma -----
  console.log(`\nCriando ${aulas.length} aulas agendadas...`);
  let criadas = 0;
  let conflitos = 0;
  let erros = 0;

  for (const [nomeProf, dia, hora, turmaCodigo, materia] of aulas) {
    const [horaInicio, horaFim] = PERIODOS[hora];
    const pId = professorId.get(nomeProf);
    const tId = turmaId.get(turmaCodigo);
    const sId = salaId.get(turmaCodigo);

    if (!pId || !tId || !sId) {
      console.error(`  Referência ausente: professor=${nomeProf} turma=${turmaCodigo}`);
      erros++;
      continue;
    }

    try {
      await api(token, 'POST', '/cronograma', {
        professorId: pId,
        turmaId: tId,
        salaId: sId,
        diaSemana: dia,
        horaInicio,
        horaFim,
        disciplina: materia,
      });
      criadas++;
    } catch (e) {
      if (e.status === 409) {
        conflitos++;
        console.warn(`  Conflito: ${nomeProf} ${dia} ${hora} (${turmaCodigo}/${materia}) — ${e.message}`);
      } else {
        erros++;
        console.error(`  Erro: ${nomeProf} ${dia} ${hora} (${turmaCodigo}/${materia}) — ${e.message}`);
      }
    }
  }

  console.log(`\n=== Resumo ===`);
  console.log(`Professores: ${professorId.size}`);
  console.log(`Turmas: ${turmaId.size} | Salas: ${salaId.size}`);
  console.log(`Aulas criadas: ${criadas}`);
  console.log(`Conflitos detectados (409): ${conflitos}`);
  console.log(`Erros: ${erros}`);
}

main().catch((e) => {
  console.error('Falha geral:', e);
  process.exit(1);
});
