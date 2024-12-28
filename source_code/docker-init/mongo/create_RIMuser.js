db.createUser({
    user: "RIMuser",
    pwd: "zaq1@WSX",
    roles: [
      {
        role: "dbAdminAnyDatabase",
        db: "admin"
      },
      {
        role: "readWriteAnyDatabase",
        db: "admin"
      }
    ]
  });