# Tailwind CSS Setup for {{ProjectName}}

This project is configured to use Tailwind CSS with automated build processes.

## Getting Started

### Prerequisites
- Node.js (v16 or later)
- npm

### Initial Setup
1. Install dependencies:
   ```bash
   npm install
   ```

2. Build CSS (one-time):
   ```bash
   npm run build-css-prod
   ```

### Development
During development, the CSS will be automatically built when you run `dotnet build` or `dotnet run`.

For faster development with CSS changes, you can run the Tailwind watcher in a separate terminal:
```bash
npm run build-css
```

This will watch for changes in your Razor files and rebuild the CSS automatically.

## File Structure

- `Styles/app.css` - Source Tailwind CSS file with your custom styles
- `wwwroot/css/app.css` - Generated CSS file (don't edit directly)
- `tailwind.config.js` - Tailwind configuration
- `postcss.config.js` - PostCSS configuration
- `package.json` - Node.js dependencies and scripts

## Customization

### Adding Custom Styles
Edit `Styles/app.css` to add custom component styles or utilities using Tailwind's `@layer` directive.

### Configuration
Modify `tailwind.config.js` to:
- Add custom colors
- Configure responsive breakpoints
- Add custom fonts
- Enable/disable plugins

### Plugins Included
- `@tailwindcss/forms` - Better form styling
- `@tailwindcss/typography` - Rich text styling
- `@tailwindcss/aspect-ratio` - Aspect ratio utilities

## Production Build
The production build automatically minifies the CSS for optimal performance.

## Troubleshooting

### CSS not updating?
1. Delete `wwwroot/css/app.css`
2. Run `npm run build-css-prod`
3. Rebuild the project

### Build errors?
1. Ensure Node.js is installed and accessible
2. Run `npm install` to reinstall dependencies
3. Check that all paths in `tailwind.config.js` are correct