'use strict';
const db = require('../../config/db')();
console.log(db);
module.exports ={getAll, save, getOne, update, deleteTeam}
function getAll(req, res, next){
    res.json({teams: db.find()});
}

function save(req, res, next){
    res.json({success: db.save(req.body), description: "Team added to list"});    
}

function getOne(req, res, next){
    let id = req.swagger.params.id.value;
    let team = db.find(id);
    if(team){
        res.json(team);
    }else{
        res.sendStatus(204);
    }
}

function update(req, res, next){
    let id = req.swagger.params.id.value;
    let team = req.body;
    if(db.update(id, team)){
        res.json({success:1, description: "Team updated"});
    }else{
        res.sendStatus(204)
    }
}

function deleteTeam(req, res, next){
    let id = req.swagger.params.id.value;
    if(db.remove(id)){
        res.json({success:1, description: "team deleted"});
    }else{
        res.sendStatus(204);
    }
}
