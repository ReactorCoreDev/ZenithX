import fs from 'fs';
import path from 'path';

function cleanObjBin(currentPath = process.cwd()) {
    for (const file of fs.readdirSync(currentPath)) {
        const fullPath = path.join(currentPath, file);
        if (fs.lstatSync(fullPath).isDirectory()) {
            if (file.toLowerCase() === 'obj' || file.toLowerCase() === 'bin') {
                fs.rmSync(fullPath, { recursive: true, force: true });
            } else {
                cleanObjBin(fullPath);
            }
        }
    }
}

cleanObjBin();
console.log('Cleanup finished!');