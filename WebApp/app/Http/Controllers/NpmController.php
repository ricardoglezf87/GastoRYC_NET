<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Artisan;

class NpmController extends Controller
{
    public function build(Request $request)
    {
        $command = $request->input('command');
        putenv("PATH=" . env("PATH") . ":/usr/local/bin/node");
        $output = shell_exec("sudo /usr/local/bin/npm run build 2>&1");
        $formattedOutput = nl2br($output);

        return view('npm_result', ['output' => $formattedOutput]);
   }
}

