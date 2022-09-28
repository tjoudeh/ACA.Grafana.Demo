import http from 'k6/http';
import {sleep} from 'k6';

const baseUrl = 'https://shipments-frontend-api.happysea-573aaf45.eastus.azurecontainerapps.io';

  export let options = {
        vus: 100,
        duration: '180s'
  };

export default function () {
    var idfrom = getRandomInt(1,10000);
    http.get(`${baseUrl}/api/shipments?idfrom=${idfrom}`);
    sleep(1);
  }

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min); // The maximum is exclusive and the minimum is inclusive
  }