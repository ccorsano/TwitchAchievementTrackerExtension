const path = require('path');
const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (_env, argv) => {
    const bundlePath = path.resolve(__dirname, "dist/")
  
    // edit webpack plugins here!
    let plugins = [
      new CleanWebpackPlugin.CleanWebpackPlugin(),
      new MiniCssExtractPlugin({
        filename: '[name].css',
      })
    ];

    let entryPoints = {
      VideoOverlay:{
        path:"./src/VideoOverlay.tsx",
        outputHtml:"video_overlay.html",
        build:true
      },
      Config:{
        path:"./src/Config.tsx",
        outputHtml:"config.html",
        build:true
      },
      Mobile:{
        path:"./src/Mobile.tsx",
        outputHtml:"mobile.html",
        build:true
      },
      //   VideoComponent:{
      //     path:"./src/VideoComponent.js",
      //     outputHtml:"video_component.html",
      //     build:true
      //   },
      //   Panel:{
      //     path:"./src/Panel.js",
      //     outputHtml:"panel.html",
      //     build:true
      //   },
      LiveConfig:{
        path:"./src/LiveConfig.tsx",
        outputHtml:"live_config.html",
        build:true
      },
    };

    let entry = {};

    for(const entryName in entryPoints){
      if(entryPoints[entryName].build){
        entry[entryName]=entryPoints[entryName].path
        // if(argv.mode==='production'){
          plugins.push(new HtmlWebpackPlugin({
            inject:true,
            chunks:[entryName],
            template:'./template.html',
            filename:entryPoints[entryName].outputHtml,
            minify: false
          }))
        // }
      }    
    }

    let config = {
        entry: entry,
        optimization: {
          minimize: true, // this setting is default to false to pass review more easily. you can opt to set this to true to compress the bundles, but also expect an email from the review team to get the full source otherwise. 
        },
        devtool: 'source-map',
        module: {
          rules: [
            {
              test: /\.tsx?$/,
              use: 'ts-loader',
              exclude: /node_modules/,
            },
            {
              test: /\.(png|jpe?g|svg)$/i,
              use: 'file-loader',
              exclude: /node_modules/,
            },
            {
              test: /\.css$/,
              use: [
                {
                  loader: MiniCssExtractPlugin.loader,
                  options: {
                    publicPath: '/dist',
                  },
                },
                'css-loader'
              ]
            },
            {
              test: /\.scss$/,
              use: [
                  "style-loader", // creates style nodes from JS strings
                  "css-loader", // translates CSS into CommonJS
                  {
                    loader: "sass-loader",
                    options: {
                      implementation: require("sass"),
                    }
                  }, // compiles Sass to CSS, using Node Sass by default
              ]
            },
          ],
        },
        resolve: {
          extensions: [ '.tsx', '.ts', '.js', '.css', '.scss' ],
        },
        output: {
          filename: '[name].bundle.js',
          path: bundlePath,
        },
        plugins: plugins,
    }

    if(argv.mode==='development'){
      config.devServer = {
        static: [
            path.resolve(__dirname, 'public')
        ],
        host:argv.devrig ? 'localhost.rig.twitch.tv' : 'localhost',
        headers: {
          'Access-Control-Allow-Origin': '*'
        },
        port: 8090,
        https: true,
        hot: true,
      }
      config.watchOptions = {
          aggregateTimeout: 1000
      };
      config.cache = {
          type: 'filesystem'
      };
      plugins.push(new webpack.HotModuleReplacementPlugin())
    }
    if(argv.mode==='production'){
      config.optimization.splitChunks={
        cacheGroups:{
          default:false,
          vendors:false,
          vendor:{
            chunks:'all',
            test:/node_modules/,
            name:false
          }
        },
        name:false
      }
    }

    return config
};