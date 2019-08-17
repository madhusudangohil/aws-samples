'use strict'
var crypto = require('crypto')

module.exports = function(){
    return {
        teamList: [],

        save(team){
            team.id = crypto.randomBytes(20).toString('hex');
            this.teamList.push(team);
            return 1;

        },
        find(id){
            if(id){
                return this.teamList.find(e=>{
                    return e.id === id;
                })
            }else{
                return this.teamList;
            }
        },
        remove(id){
            var found = 0;
            this.teamList = this.teamList.filter(e=>{
                if(e.id === id){
                    found = 1;
                }else{
                    return e.id !== id;
                }
            });
            return found;
        },
        update(id, team){
            var teamIndex = this.teamList.findIndex(e=>{
                return e.id === id;
            });
            if(teamIndex !== -1){
                this.teamList[teamIndex].captain = team.captain;
                this.teamList[teamIndex].country = team.country;
                return 1;
            }else{
                return 0;
            }

        }
    }
}