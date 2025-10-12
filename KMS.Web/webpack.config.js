const path = require("path");
const TerserPlugin = require("terser-webpack-plugin");
const { WebpackManifestPlugin } = require("webpack-manifest-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");

module.exports = (env, argv) => {
    const isDevelopment = argv.mode === "development";

    return {
        mode: isDevelopment ? "development" : "production",

        entry: {
            // Pages
            home: "./wwwroot/js/pages/home/index.js",
            chatbot: "./wwwroot/js/pages/chatbot/index.js",
            search: "./wwwroot/js/pages/search-page/index.js",
            "document-detail": "./wwwroot/js/pages/document-detail/index.js",
            "document-borrow": "./wwwroot/js/pages/document-borrow/index.js",

            // Components (có thể shared giữa nhiều pages)
            header: "./wwwroot/js/components/header/index.js",
            "advanced-search":
                "./wwwroot/js/components/advanced-search/index.js",
            "search-bar": "./wwwroot/js/components/search-bar/index.js",
        },

        output: {
            path: path.resolve(__dirname, "wwwroot/public"),
            filename: "js/[name].[contenthash].js",
            publicPath: "/",
        },

        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                        options: {
                            presets: [
                                [
                                    "@babel/preset-env",
                                    {
                                        modules: false,
                                        targets: "defaults",
                                    },
                                ],
                            ],
                            plugins: ["@babel/plugin-syntax-dynamic-import"],
                            sourceType: "unambiguous",
                        },
                    },
                    type: "javascript/auto",
                },
                {
                    test: /\.css$/i,
                    use: [
                        {
                            loader: MiniCssExtractPlugin.loader,
                            options: {
                                publicPath: "../", // cái này hợp lệ ở MiniCssExtractPlugin.loader
                            },
                        },
                        {
                            loader: "css-loader",
                            options: {
                                url: true, // cho phép xử lý url() trong CSS
                                import: true,
                            },
                        },
                    ],
                },
                {
                    test: /\.(png|jpe?g|gif|svg|webp)$/i,
                    type: "asset/resource",
                    generator: {
                        // Xuất ra thư mục images nằm ngang hàng với js/css
                        filename: "img/[hash][ext][query]",
                    },
                },
                {
                    test: /\.(woff2?|eot|ttf|otf)$/i,
                    type: "asset/resource",
                    generator: {
                        filename: "fonts/[name][hash][ext][query]", // font vào wwwroot/public/fonts
                    },
                },
            ],
        },

        resolve: {
            extensions: [".js", ".json"],
            alias: {
                "@": path.resolve(__dirname, "wwwroot/js"),
                "@common": path.resolve(__dirname, "wwwroot/js/common"),
                "@components": path.resolve(__dirname, "wwwroot/js/components"),
                "@utils": path.resolve(__dirname, "wwwroot/js/utils"),
                "@states": path.resolve(__dirname, "wwwroot/js/states"),
                "@services": path.resolve(__dirname, "wwwroot/js/services"),
                "@pages": path.resolve(__dirname, "wwwroot/js/pages"),
                "@libs": path.resolve(__dirname, "wwwroot/js/libs"),
            },
        },

        optimization: {
            minimize: !isDevelopment,
            minimizer: [
                new TerserPlugin({
                    extractComments: false, // Không tạo file .LICENSE.txt
                    terserOptions: {
                        compress: {
                            drop_console: !isDevelopment, // Chỉ remove console.log ở production
                        },
                        format: {
                            comments: false, // Bỏ comment trong bundle
                        },
                    },
                }),
                new CssMinimizerPlugin(),
            ],

            // Code splitting để optimize loading
            // splitChunks: {
            //     chunks: "all",
            //     cacheGroups: {
            //         // Vendor libraries (nếu có)
            //         vendor: {
            //             test: /[\\/]node_modules[\\/]/,
            //             name: "vendors",
            //             chunks: "all",
            //             priority: 20,
            //         },

            //         // Common utilities (được dùng >= 2 lần)
            //         common: {
            //             test: /[\\/]js[\\/](common|utils|services)[\\/]/,
            //             name: "common",
            //             chunks: "all",
            //             minChunks: 2,
            //             priority: 10,
            //             reuseExistingChunk: true,
            //         },

            //         // Shared components
            //         components: {
            //             test: /[\\/]js[\\/]components[\\/]/,
            //             name: "shared-components",
            //             chunks: "all",
            //             minChunks: 2,
            //             priority: 5,
            //             reuseExistingChunk: true,
            //         },
            //     },
            // },
        },

        // Source maps cho development
        devtool: isDevelopment ? "eval-source-map" : false,

        // Stats output
        stats: {
            colors: true,
            modules: false,
            children: false,
            chunks: isDevelopment ? false : true,
            chunkModules: false,
        },

        // Performance hints
        performance: {
            hints: isDevelopment ? false : "warning",
            maxEntrypointSize: 512000,
            maxAssetSize: 512000,
        },

        plugins: [
            new CleanWebpackPlugin({
                cleanOnceBeforeBuildPatterns: [
                    path.resolve(__dirname, "wwwroot/public/js/*"),
                    path.resolve(__dirname, "wwwroot/public/css/*"),
                ],
            }),
            new WebpackManifestPlugin({
                generate: (seed, files) => {
                    const manifest = {};

                    files.forEach((file) => {
                        const filename = path.basename(file.path);

                        manifest[file.name] = filename;
                    });

                    return manifest;
                },
                fileName: path.resolve(
                    __dirname,
                    "wwwroot/public/manifest/manifest.json"
                ),
                publicPath: "/",
                filter: (file) => file.isInitial, // chỉ lấy file chính
                map: (file) => {
                    // bỏ prefix path dài, chỉ lấy tên gốc
                    file.name = file.name.replace(/\.js$/, "") + ".js";
                    return file;
                },
            }),
            new MiniCssExtractPlugin({
                filename: "css/[name].[contenthash].css",
            }),
        ],
    };
};
