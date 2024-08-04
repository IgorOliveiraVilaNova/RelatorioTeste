db = db.getSiblingDB("ProjetoBTG"); // Substitua "yourDatabaseName" pelo nome do seu banco de dados

// Cria a coleção "pedidos"
if (!db.getCollectionNames().includes("pedidos")){

    db.createCollection("pedidos");
    // Cria o índice para "codigoPedido"
    db.pedidos.createIndex({ "codigoPedido": 1 });
    
    // Cria o índice para "codigoCliente"
    db.pedidos.createIndex({ "codigoCliente": 1 });
}

